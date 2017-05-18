namespace nec.nexu.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempIdDesignLocation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DesignLocations", "Template_Id", "dbo.ProductTemplates");
            DropIndex("dbo.DesignLocations", new[] { "Template_Id" });
            RenameColumn(table: "dbo.DesignLocations", name: "Template_Id", newName: "TemplateId");
            AlterColumn("dbo.DesignLocations", "TemplateId", c => c.Int(nullable: false));
            CreateIndex("dbo.DesignLocations", "TemplateId");
            AddForeignKey("dbo.DesignLocations", "TemplateId", "dbo.ProductTemplates", "Id", cascadeDelete: true);
            DropColumn("dbo.DesignLocations", "TemplateViewId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DesignLocations", "TemplateViewId", c => c.Int(nullable: false));
            DropForeignKey("dbo.DesignLocations", "TemplateId", "dbo.ProductTemplates");
            DropIndex("dbo.DesignLocations", new[] { "TemplateId" });
            AlterColumn("dbo.DesignLocations", "TemplateId", c => c.Int());
            RenameColumn(table: "dbo.DesignLocations", name: "TemplateId", newName: "Template_Id");
            CreateIndex("dbo.DesignLocations", "Template_Id");
            AddForeignKey("dbo.DesignLocations", "Template_Id", "dbo.ProductTemplates", "Id");
        }
    }
}
