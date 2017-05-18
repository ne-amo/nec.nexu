using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace nec.nexu.Models
{
    public class TokenHelper : IDisposable
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public AppClient FindClient(string clientId)
        {
            var client = db.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

            var existingToken = db.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            db.RefreshTokens.Add(token);

            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await db.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                db.RefreshTokens.Remove(refreshToken);
                return await db.SaveChangesAsync() > 0;
            }

            return false;
        }
        public async Task<bool> RemoveRefreshToken(string user, string appId)
        {
            var refreshToken = db.RefreshTokens.Where(x => x.Subject == user && x.ClientId == appId).FirstOrDefault();
        
            if (refreshToken != null)
            {
                db.RefreshTokens.Remove(refreshToken);
                return await db.SaveChangesAsync() > 0;
            }
        
            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            db.RefreshTokens.Remove(refreshToken);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await db.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return db.RefreshTokens.ToList();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public string GetHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }


        public void CreateClient(AppClient clientJson)
        {
            if (db.Clients.Find(clientJson.Id) != null)
            {
                throw new Exception("Client Id already exists");
            }

            AppClient client = new AppClient();
            client.Active = true;
            client.AllowedOrigin = clientJson.AllowedOrigin;
            client.ApplicationType = clientJson.ApplicationType;
            client.Id = clientJson.Id;
            client.Name = clientJson.Name;
            client.RefreshTokenLifeTime = clientJson.RefreshTokenLifeTime;
            client.Secret = this.GetHash(clientJson.Secret);

            db.Clients.Add(client);

            db.SaveChanges();

        }

  
    }

}