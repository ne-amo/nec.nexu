using System;
using System.Collections.Generic;
using System.Linq;
using Nec.Infrastructure.Messages;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.IO;
//using nec.rendering;

namespace nec.nexu.bridge
{
    public class Bridge : IDisposable
    {
        #region Properties and Constructors
        private bool disposed = false;
        private readonly RoalServiceClient roal;
        private readonly SqlConnection connection;
        private readonly bool disposeRoal = false;
        private string apiUrl = "api/roal/{0}";
        //private ApiConnector nexu = new ApiConnector();
        private string _sqlBasicData
        {
            get
            {
                return @"
                            select top 1
	                            a.sbu 					    as 'StrategicBusinessUnitCode'
	                            ,a.Prod_Desc 			    as 'StrategicBusinessUnitDescription'
	                            ,b.gender				    as 'GenderCode'
	                            ,b.Prod_Desc				as 'GenderDescription'
	                            ,c.silhouette			    as 'SilhouetteCode'
	                            ,c.Prod_Desc				as 'SilhouetteDescription'
	                            ,d.sport					as 'SportCode'
	                            ,d.Prod_Desc				as 'SportDescription'
	                            ,e.license				    as 'LicenseCode'
	                            ,e.Prod_Desc				as 'LicenseDescription'
	                            ,f.team					    as 'TeamCode'
	                            ,f.Prod_Desc				as 'TeamDescription'
	                            ,g.[class]				    as 'ClassCode'
	                            ,g.Prod_Desc				as 'ClassDescription'
                            from [db_owner].[vw_PH_Level2] a (nolock)
                            full outer join  [db_owner].[vw_PH_Level3] b (nolock) on 1=1
                            full outer join  [db_owner].[vw_PH_Level4] c (nolock) on 1=1
                            full outer join  [db_owner].[vw_PH_Level5] d (nolock) on 1=1
                            full outer join  [db_owner].[vw_PH_Level6] e (nolock) on 1=1
                            full outer join  [db_owner].[vw_PH_Level7] f (nolock) on 1=1
                            full outer join  [db_owner].[vw_PH_Level8] g (nolock) on 1=1
                            where
	                                a.sbu 			= @a			
	                            and b.gender		= @b		
	                            and c.silhouette	= @c		
	                            and d.sport			= @d		
	                            and e.license		= @e		
	                            and f.team			= @f		
	                            and g.[class]		= @g		";
            }
        }
        private string _sqlTeamCode
        {
            get
            {
                return "select top 1 [TeamCode] from [ROAL].[dbo].[TeamCodes] (nolock) where [TeamId] = @a";
            }
        }
        private ColorListMessage[] allColors;




        public Bridge()
        {
            this.roal = new RoalServiceClient("ws");
            this.connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["roal"].ToString());
        }
        public Bridge(RoalServiceClient Roal, bool DisposeRoal)
        {
            this.roal = Roal;
            this.connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["roal"].ToString());
        }
        public Bridge(SqlConnection Connection)
        {
            this.roal = new RoalServiceClient("ws");
            this.connection = Connection;
        }
        public Bridge(SqlConnection Connection, RoalServiceClient Roal, bool DisposeRoal)
        {
            this.roal = Roal;
            this.connection = Connection;
        }


        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.connection.Close();
                this.connection.Dispose();
                //this.nexu.Dispose();
                if (this.disposeRoal)
                {
                    roal.Close();
                }
            }
            this.Dispose(disposing);
        }



        #endregion

        #region Public Methods


        public int Create(int materialId)
        {
            return this.create(materialId);
        }

        public TicketMessage GetMaterial(int materialId)
        {
            NexuToRoalMessage n = NexuRequestObject<NexuToRoalMessage>(string.Format(apiUrl, materialId));
            if (n == null)
                throw new Exception("Could not find material " + materialId);

            try
            {
                TicketMessage ticket = new TicketMessage();
                ticket.MaterialId = materialId;
                ConfigurationCollectionMessage config = roal.FindConfigurationCollection(n.ConfigurationId);
                IllustrationMessage[] designs = this.getIllustrations(n);
                ticket = build(n, ticket, config,designs);
                return ticket;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region Private Methods

        private int create(int materialId)
        {
            NexuToRoalMessage n = NexuRequestObject<NexuToRoalMessage>(string.Format(apiUrl, materialId));
            if (n == null)
                throw new Exception("Could not find material " + materialId);

            TicketMessage ticketBuild = new TicketMessage();
            TicketMessage ticketCreated = new TicketMessage();
            ticketBuild.MaterialId = materialId;
            ConfigurationCollectionMessage config = roal.FindConfigurationCollection(n.ConfigurationId);
            IllustrationMessage[] designs = this.getIllustrations(n);
            try
            {
                // Build Ticket
                ticketBuild = build(n, ticketBuild, config, designs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error building " + materialId, ex);
            }
            try
            {
                ticketCreated = roal.CreateTicket(ticketBuild);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating " + materialId, ex);
            }
            try
            {
                // Render material
                ticketBuild.FlippedRegions = new Dictionary<RegionFlipRuleMessage, bool>();
                Dictionary<int, int> embroideries = getEmbroideryId(ticketCreated);
                foreach (EmbroideryMessage emb in ticketBuild.Embroideries)
                {
                    emb.Id = embroideries[(int)emb.ConfigurationIllustrationId];
                }
                //new nec.rendering.Render(ticketBuild, config, designs);
            }
            catch (Exception ex)
            {
                //throw new Exception("Error rendering " + materialId, ex);
            }

            return (int)ticketCreated.Id;
        }

        private TicketMessage build(NexuToRoalMessage n, TicketMessage ticket, ConfigurationCollectionMessage config, IllustrationMessage[] designs)
        {
            int id = ticket.MaterialId;
            /* Build basic message */
            try
            {
                ticket = formatBasicData(ticket,n);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on basic data of material " + id, ex);
            }
            /* Build regions*/
            try
            {
                ticket = formatRegionData(ticket, n, config.Regions);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on regions of material " + id, ex);
            }
            /* Build stitches*/
            try
            {
                ticket = formatStitchData(ticket, n, config.Stitches);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on stitches of material " + id, ex);
            }
            /* Build Components */
            try
            {
                ticket = formatComponents(ticket, n, config.Components);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on logo of material " + id, ex);
            }
            /* Build logos */
            try
            {
                this.allColors = roal.FindColorListByExample(new ColorListMessage { ColorGroup = 2 });
                ticket = formatEmbroideries(ticket, n, config.Illustrations,designs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on logo of material " + id, ex);
            }
            return ticket;
        }


        private class AccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string userName { get; set; }
        }

        private static async Task<string> TokenRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["api_url"]+"/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "client_id", ConfigurationManager.AppSettings["api_client_id"] },
                    { "client_secret", ConfigurationManager.AppSettings["api_client_secret"] },
                    { "grant_type", "client_credentials" }
                });
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var payload = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessToken>(await response.Content.ReadAsStringAsync());
            var token = payload.access_token;
            return token;
        }

        private static string NexuRequest(string endpoint)
        {
            string responseFromServer = String.Empty;
            try
            {
                //string token = TokenRequest().Result;
                string base_url = ConfigurationManager.AppSettings["api_url"];
                WebRequest request = WebRequest.Create(Path.Combine(base_url, endpoint));

                //var myHttpWebRequest = (HttpWebRequest)request;
                //myHttpWebRequest.PreAuthenticate = true;
                //myHttpWebRequest.Headers.Add("Authorization", "Bearer " + token);
                //myHttpWebRequest.Accept = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseFromServer;
        }

        private T NexuRequestObject<T>(string endpoint)
        {
            string response = NexuRequest(endpoint);
            if (String.IsNullOrWhiteSpace(response))
            {
                return default(T);
            }
            else
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(NexuRequest(endpoint));
            }
        }


        private TicketMessage formatBasicData(TicketMessage ticket, NexuToRoalMessage msg)
        {
            TicketMessage newtm = new TicketMessage();
            Dictionary<string, string> data = this.getBasicData(msg);
            // Change TicketNew Information
            ticket.SalesRepresentativeCode = ConfigurationManager.AppSettings["default_UserId"];
            ticket.SalesRepresentativeName = ConfigurationManager.AppSettings["default_UserName"];
            ticket.SeasonCode = msg.SeasonId;
            ticket.ProductGroupCode = ConfigurationManager.AppSettings["default_ProductGroupId"];
            ticket.DesignTeam = ConfigurationManager.AppSettings["default_DesignTeamId"];
            ticket.StrategicBusinessUnitCode = data["StrategicBusinessUnitCode"];
            ticket.StrategicBusinessUnitDescription = data["StrategicBusinessUnitDescription"];
            ticket.GenderCode = data["GenderCode"];
            ticket.GenderDescription = data["GenderDescription"]; 
            ticket.SilhouetteCode = data["SilhouetteCode"];
            ticket.SilhouetteDescription = data["SilhouetteDescription"];
            ticket.SportCode = data["SportCode"];
            ticket.SportDescription = data["SportDescription"];
            ticket.LicenseCode = data["LicenseCode"];
            ticket.LicenseDescription = data["LicenseDescription"];
            ticket.TeamCode = data["TeamCode"];
            ticket.TeamDescription = data["TeamDescription"];
            ticket.ClassCode = data["ClassCode"];
            ticket.ClassDescription = data["ClassDescription"];
            ticket.RequestDate = DateTime.Now;
            ticket.DueDate = DateTime.Now;
            ticket.VerifyDate = DateTime.Now;
            ticket.Status = 0;
            ticket.Mainlandable = true;
            ticket.Active = true;
            ticket.ConfigurationCollectionId = msg.ConfigurationId;
            ticket.SeasonCollectionCode = msg.CollectionId;
            ticket.ColorOptionId = long.Parse(ConfigurationManager.AppSettings["default_ColorOptionId"]);
            ticket.TicketSourceId = 1;
            ticket.ByPassDuplicateCheck = false;
            ticket.RequestTypeId = 3;
            ticket.MaterialDescription = string.Format("{0} {1}",ticket.SilhouetteDescription,this.getMaterialDescription(msg).ToUpper());
            // Add Country Code to Comments - amo 2017.01.24
            ticket.AdditionalFeatures = msg.CountryCode;
            return ticket;
        }
        private Dictionary<string, string> getBasicData(NexuToRoalMessage msg)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string sql = _sqlBasicData;
            SqlCommand command = new SqlCommand(sql, this.connection);
            command.Parameters.AddWithValue("@a", msg.BuCode);
            command.Parameters.AddWithValue("@b", msg.GenderId);
            command.Parameters.AddWithValue("@c", msg.SilhouetteCode);
            command.Parameters.AddWithValue("@d", msg.SportCode);
            command.Parameters.AddWithValue("@e", msg.LicenseCode);
            command.Parameters.AddWithValue("@f", msg.TeamCode);
            command.Parameters.AddWithValue("@g", msg.ClassCode);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        data.Add(reader.GetName(i), reader[i].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return data;
        }
        private string getMaterialDescription(NexuToRoalMessage msg)
        {
            string sql = _sqlTeamCode;
            SqlCommand command = new SqlCommand(sql, this.connection);
            command.Parameters.AddWithValue("@a", msg.TeamCode);
            string teamcode = "NEXU ";
            try
            {
                connection.Open();
                teamcode += command.ExecuteScalar().ToString();

            }
            catch (Exception ex)
            {
                throw new Exception("Error getting team code", ex);
            }
            finally
            {
                connection.Close();
            }
            switch (msg.ClassCode)
            {
                case "08":
                    return teamcode +"CO";
                case "06":
                    return teamcode + "HC";
                case "07":
                    return teamcode+"VC";
                default:
                    return teamcode;
            }
        }

        private TicketMessage formatRegionData(TicketMessage ticket, NexuToRoalMessage msg, ConfigurationRegionMessage[] config)
        {
            List<RegionMessage> regions = new List<RegionMessage>();
            foreach (ConfigurationRegionMessage cfgRegion in config)
            {
                try
                {
                    string materialId = null;
                    if(msg.RegionMaterials.ContainsKey((int)cfgRegion.Id))
                        materialId = msg.RegionMaterials[(int)cfgRegion.Id];
                    
                    if(string.IsNullOrEmpty(materialId))
                        throw new Exception("NEXU setting not found");

                    MaterialMessage material = roal.FindMaterialByExample(new MaterialMessage { Code = materialId }).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Fabric not found");

                    var cutPiece = roal.FindDefaultRegionMaterials(new DefaultRegionRequestMessage
                    {
                         ConfigurationCollectionId = ticket.ConfigurationCollectionId,
                         DefaultMaterial = material
                    }).FirstOrDefault(x=>x.Configuration.Id == cfgRegion.Id);

                    RegionMessage region = null;

                    if (cutPiece != null)
                    {
                        region = cutPiece;
                    }
                    else
                    {
                        region = new RegionMessage
                        {
                            Configuration = cfgRegion,
                            TicketId = 0,
                            MaterialCode = material.Code,
                            MaterialColorId = material.MaterialColor.Id,
                            MaterialColorHex = material.MaterialColor.Hex,
                            MaterialColorPms = material.MaterialColor.Pms,
                            MaterialColorRgb = material.MaterialColor.Rgb,
                            MaterialColorPatternPath = material.MaterialColor.PatternPath,
                            MaterialFcc = material.Fcc,
                            Description = material.Description
                        };
                    }
                    regions.Add(region);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error building region for Material " + ticket.MaterialId, ex);
                }
            }
            ticket.Regions = regions.ToArray();
            return ticket;
        }

        private string isCapSolid(TicketMessage ticket)
        {
            string capIsSolid = string.Empty;
            long configId = ticket.ConfigurationCollectionId;
            var crownIds = getCrownRegionIds(configId);

            if(!crownIds.Any()) // data not setup, use mono to be safe.
                return string.Empty;

            long color = ticket.Regions.First().MaterialColorId;
            bool solid = true;
            foreach (var id in crownIds)
            {
                var region = ticket.Regions.FirstOrDefault(x => x.Configuration.Id == id);
                if (region != null)
                {
                    if (region.MaterialColorId != color)
                    {
                        solid = false;
                        break;
                    }
                }
            }
            if (solid)
                capIsSolid = ticket.Regions.First().MaterialCode;

            return capIsSolid;
        }

        private TicketMessage formatStitchData(TicketMessage ticket, NexuToRoalMessage msg, ConfigurationStitchMessage[] config)
        {
            List<StitchMessage> regions = new List<StitchMessage>();
            long[] monoRegions = ConfigurationManager.AppSettings["forceMono"].Split(',').Select(c=>long.Parse(c)).ToArray();
            foreach (ConfigurationStitchMessage cfgRegion in config)
            {
                try
                {
                    if (monoRegions.Contains(cfgRegion.Id))
                    {
                        var solidCapMaterialId = isCapSolid(ticket);
                        if (!string.IsNullOrEmpty(solidCapMaterialId))
                        {
                            // get the matching stitch color
                            Dictionary<int, string> reqKey = new Dictionary<int, string>();
                            reqKey.Add((int)cfgRegion.Id, solidCapMaterialId);

                            StitchMatchRequestMessage req = new StitchMatchRequestMessage
                            {
                                StitchType = (cfgRegion.SapHierarchyCode == "030N015") ? 1 : 0,
                                SapHierarchyCodes = reqKey,
                                ConfigurationCollection = new ConfigurationCollectionMessage()
                                {
                                    Id = ticket.ConfigurationCollectionId,
                                    Version = 0,
                                    RenderCode = 0,
                                    SortOrder = 0,
                                    DefaultRegionSapHierarchyCode = "0315"
                                }
                            };
                            StitchMessage stitch = roal.FindMaterialStitchMatches(req).FirstOrDefault();
                            if(stitch != null)
                            {
                                regions.Add(stitch);
                            }
                            else
                            {
                                regions.Add(getMonoStitch(cfgRegion));
                            }
                        }
                        else
                        {
                            regions.Add(getMonoStitch(cfgRegion));
                        }
                    }
                    else
                    {


                        string fabricMaterialId = null;
                        if (msg.StitchMaterials.ContainsKey((int)cfgRegion.Id))
                        {
                            fabricMaterialId = msg.StitchMaterials[(int)cfgRegion.Id];
                        }
                        if (string.IsNullOrEmpty(fabricMaterialId))
                        {
                            throw new Exception(string.Format("Mapped value not found for {0}",cfgRegion.Id));
                        }
                            
                        



                        MaterialMessage stitch = roal.FindMaterialByExample(new MaterialMessage { Code = fabricMaterialId }).FirstOrDefault();
                        if (stitch == null)
                            throw new Exception("Could not find stitch material.");


                        StitchMessage newmsg = new StitchMessage();
                        newmsg.TicketId = 0;
                        newmsg.Configuration = cfgRegion;
                        newmsg.ColorRgb = stitch.MaterialColor.Rgb;
                        newmsg.Description = stitch.Description;
                        newmsg.Fcc = stitch.Fcc;
                        newmsg.MaterialCode = stitch.Code;
                        newmsg.MaterialColorHex = stitch.MaterialColor.Hex;
                        newmsg.MaterialColorId = stitch.MaterialColor.Id;
                        newmsg.MaterialColorPms = stitch.MaterialColor.Pms;
                        newmsg.MaterialColorRgb = stitch.MaterialColor.Rgb;
                        newmsg.MaterialFcc = stitch.Fcc;
                        newmsg.Version = 0;

                        //StitchMessage newmsg = new StitchMessage();
                        //newmsg.TicketId = 0;
                        //newmsg.Configuration = cfgRegion;
                        //newmsg.ColorRgb = stitch.ColorRgb;
                        //newmsg.Description = stitch.Description;
                        //newmsg.Fcc = stitch.Fcc;
                        //newmsg.MaterialCode = stitch.MaterialCode;
                        //newmsg.MaterialColorHex = stitch.MaterialColorHex;
                        //newmsg.MaterialColorId = stitch.MaterialColorId;
                        //newmsg.MaterialColorPms = stitch.MaterialColorPms;
                        //newmsg.MaterialColorRgb = stitch.MaterialColorRgb;
                        //newmsg.MaterialFcc = stitch.MaterialFcc;
                        //newmsg.Version = 0;

                        regions.Add(newmsg);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error building stitch for Material " + ticket.MaterialId, ex);
                }
            }
            ticket.Stitches = regions.ToArray();
            return ticket;
        }
        private StitchMessage getMonoStitch(ConfigurationStitchMessage config)
        {
            MaterialMessage material = roal.FindMaterialByExample(new MaterialMessage { Code = ConfigurationManager.AppSettings["default_MonoThread"] }).FirstOrDefault();
            if (material == null)
                throw new Exception("Mono not found");

            StitchMessage region = new StitchMessage
            {
                Configuration = config,
                TicketId = 0,
                MaterialCode = material.Code,
                MaterialColorId = material.MaterialColor.Id,
                MaterialColorHex = material.MaterialColor.Hex,
                MaterialColorPms = material.MaterialColor.Pms,
                MaterialColorRgb = material.MaterialColor.Rgb,
                MaterialFcc = material.Fcc,
                Description = material.Description
            };
            return region;
        }

        private IllustrationMessage[] getIllustrations(NexuToRoalMessage msg)
        {
            List<IllustrationMessage> list = new List<IllustrationMessage>();
            foreach (NexuToRoalMessage.DesignSetting setting in msg.Designs)
            {
                IllustrationMessage design = roal.FindIllustration(setting.DesignId);
                list.Add(design);
            }
            return list.ToArray();
        }
        private TicketMessage formatEmbroideries(TicketMessage ticket, NexuToRoalMessage msg, ConfigurationIllustrationMessage[] config, IllustrationMessage[] designs)
        {
            List<EmbroideryMessage> list = new List<EmbroideryMessage>();
            try
            {
                foreach (NexuToRoalMessage.DesignSetting setting in msg.Designs)
                {
                    ConfigurationIllustrationMessage cfg = config.FirstOrDefault(x => x.Id == setting.LocationId);
                    if (cfg == null)
                        throw new Exception("Could not find configuration illustration");
                    IllustrationMessage design = designs.First(x=>x.Id == setting.DesignId);
                    if(design == null)
                        throw new Exception("Could not find design");

                    EmbroideryMessage embroidery = new EmbroideryMessage{
                          ConfigurationIllustrationId = cfg.Id,
                          IllustrationId = design.Id,
                          ManualIllustrationNumber = design.ManualIllustrationNumber,
                          TicketId = 0,
                          Version = 0
                    };
                    ColorwayMessage colorway = new ColorwayMessage
                    {
                        IllustrationId = (int)design.Id,
                        IsDefault = false
                    };

                    ColorwayMessage defaultColorway = roal.FindColorwayByExample(new ColorwayMessage { IllustrationId = (int)design.Id, IsDefault = true}).FirstOrDefault();
                    if(defaultColorway == null)
                        throw new Exception("Could not find default colorway");

                    List<ColorSequenceMessage> sequences = new List<ColorSequenceMessage>();
                    foreach (ColorSequenceMessage sequence in defaultColorway.ColorSequences)
                    {
                        int colorListId = setting.Settings.First(x => x.Key == sequence.SortOrder).Value;
                        ColorListMessage color = allColors.First(x=>x.Id == colorListId);
                        ColorSequenceMessage newSequence = new ColorSequenceMessage
                        {
                              ColorList = color,
                              ColorType = sequence.ColorType,
                              SortOrder = sequence.SortOrder,
                              Region = sequence.Region,
                              ProductionValue = sequence.ProductionValue,
                              Version = 0,
                              Id = 0
                        };
                        sequences.Add(newSequence);
                    }
                    colorway.ColorSequences = sequences.ToArray();
                    embroidery.Colorway = colorway;
                    list.Add(embroidery);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing logo",ex);
            }
            ticket.Embroideries = list.ToArray();
            return ticket;
        }
        private Dictionary<int, int> getEmbroideryId(TicketMessage ticket)
        {
            Dictionary<int, int> data = new Dictionary<int, int>();

            SqlCommand command = new SqlCommand("select ConfigurationIllustrationId, Id from TicketEmbroidery (nolock) where ticketid = @a",connection);
            command.Parameters.AddWithValue("@a", ticket.Id);
            try
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add((int)reader[0], (int)reader[1]);
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return data;
        }

        private List<int> getCrownRegionIds(long configId)
        {
            List<int> data = new List<int>();

            SqlCommand command = new SqlCommand("select [configuration_region_id] from [configuration_crowns] (nolock) where [configuration_collection_id] = @a", connection);
            command.Parameters.AddWithValue("@a", configId);
            try
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader[0] != DBNull.Value)
                            data.Add((int)reader[0]);
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return data;
        }

        private TicketMessage formatComponents(TicketMessage ticket, NexuToRoalMessage msg, ConfigurationComponentMessage[] config)
        {
            List<ComponentMessage> list = new List<ComponentMessage>();
            foreach (ConfigurationComponentMessage c in config.Where(x => !x.ComponentRuleTypeId.HasValue))
            {
                if (!string.IsNullOrEmpty(c.DefaultMaterialId))
                {
                    // Load default material
                    MaterialMessage material = roal.FindMaterialByExample(new MaterialMessage { Code = c.DefaultMaterialId }).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Default component not found: "+c.DefaultMaterialId);
                    ComponentMessage comp = new ComponentMessage
                    {
                        Configuration = c,
                        Description = material.Description,
                        MaterialCode = material.Code,
                        TicketId = 0
                    };
                    list.Add(comp);
                }
                else
                {
                    // Find one form the NEXU settings
                    string materialId = msg.ComponentMaterials[(int)c.Id];
                    if (string.IsNullOrEmpty(materialId))
                        continue;

                    MaterialMessage material = roal.FindMaterialByExample(new MaterialMessage { Code = materialId }).FirstOrDefault();
                    if (material == null)
                        throw new Exception("Fabric not found");

                    ComponentMessage comp = new ComponentMessage
                    {
                        Configuration = c,
                        Description = material.Description,
                        MaterialCode = material.SapMaterialId,
                        TicketId = 0
                    };
                    list.Add(comp);
                }
            }
            ticket.Components = list.ToArray();
            return ticket;
        }

        #endregion

    }
}
