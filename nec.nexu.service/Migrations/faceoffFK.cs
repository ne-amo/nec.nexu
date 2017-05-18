namespace nec.nexu.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceoffFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FaceOffs", "FinishedProductA_Id", "dbo.FinishedProducts");
            DropForeignKey("dbo.FaceOffs", "FinishedProductB_Id", "dbo.FinishedProducts");
            DropIndex("dbo.FaceOffs", new[] { "FinishedProductA_Id" });
            DropIndex("dbo.FaceOffs", new[] { "FinishedProductB_Id" });
            AddColumn("dbo.FaceOffs", "FinishedProductAId", c => c.Int());
            AddColumn("dbo.FaceOffs", "FinishedProductBId", c => c.Int());
            AlterColumn("dbo.FinishedProducts", "InspiredByFinishedProductId", c => c.Int());
            AlterColumn("dbo.FaceOffs", "FinishedProductAId", c => c.Int(nullable: true));
            AlterColumn("dbo.FaceOffs", "FinishedProductBId", c => c.Int(nullable: true));
            CreateIndex("dbo.FaceOffs", "FinishedProductAId");
            CreateIndex("dbo.FaceOffs", "FinishedProductBId");
            AddForeignKey("dbo.FaceOffs", "FinishedProductAId", "dbo.FinishedProducts", "Id");
            AddForeignKey("dbo.FaceOffs", "FinishedProductBId", "dbo.FinishedProducts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FaceOffs", "FinishedProductB_Id1", "dbo.FinishedProducts");
            DropForeignKey("dbo.FaceOffs", "FinishedProductA_Id1", "dbo.FinishedProducts");
            DropIndex("dbo.FaceOffs", new[] { "FinishedProductB_Id1" });
            DropIndex("dbo.FaceOffs", new[] { "FinishedProductA_Id1" });
            AlterColumn("dbo.FaceOffs", "FinishedProductB_Id", c => c.Int());
            AlterColumn("dbo.FaceOffs", "FinishedProductA_Id", c => c.Int());
            AlterColumn("dbo.FinishedProducts", "InspiredByFinishedProductId", c => c.Int(nullable: false));
            DropColumn("dbo.FaceOffs", "FinishedProductB_Id1");
            DropColumn("dbo.FaceOffs", "FinishedProductA_Id1");
            CreateIndex("dbo.FaceOffs", "FinishedProductB_Id");
            CreateIndex("dbo.FaceOffs", "FinishedProductA_Id");
            AddForeignKey("dbo.FaceOffs", "FinishedProductB_Id", "dbo.FinishedProducts", "Id");
            AddForeignKey("dbo.FaceOffs", "FinishedProductA_Id", "dbo.FinishedProducts", "Id");
        }
    }
}
