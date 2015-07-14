using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniDbDeploy
{
    class Configuration
    {
        public enum DdlLogging
        {
            none,
            changes,
            all 
        }

        public Int64 id;
        //public Int64 targetId;
        public Int64 type;
        public Int64 tableSelectSchema;
        public Int64 tableSelectData;
        public string title;
        public string transferStaticData;
        public string transferTypesData;
        public long transferMaxRows = 0;
        //TODO: 0. Allow this to edited and pass in constructor
        public string backupPerform = "Y";
        //TODO: 0. Allow this to edited and pass in constructor
        public string backupLocation = "C:\\";
        //TODO: 0. Allow this to edited and pass in constructor
        public DdlLogging ddlLogging = DdlLogging.changes;
        public string tempExcludeSchema = "Y";
        public string tempExcludeData = "Y";
        public Dictionary<string, TableSelect> schemaTables = new Dictionary<string, TableSelect>();
        public Dictionary<string, TableSelect> dataTables = new Dictionary<string, TableSelect>();
        public Dictionary<string, TableFilter> filterTables = new Dictionary<string, TableFilter>();
        public string FKCheckSync = "Y";

        public override string ToString()
        {
            return this.title;
        }

        public Configuration()
        {
        }

        public Configuration(
            Int64 id, 
            //Int64 targetId, 
            Int64 type, 
            Int64 tableSelectSchema, 
            Int64 tableSelectData, 
            string title, 
            string transferStaticData, 
            string transferTypesData,
            long transferMaxRows,
            DdlLogging ddlLogging,
            string tempExcludeSchema,
            string tempExcludeData,
            string backupPerform,
            string backupLocation,
            string FKCheckSync
            )
        {
            this.id = id;
            //this.targetId = targetId;
            this.type = type;
            this.tableSelectSchema = tableSelectSchema;
            this.tableSelectData = tableSelectData;
            this.title = title;
            this.transferStaticData = transferStaticData;
            this.transferTypesData = transferTypesData;
            this.transferMaxRows = transferMaxRows;
            this.ddlLogging = ddlLogging;
            this.tempExcludeSchema = tempExcludeSchema;
            this.tempExcludeData = tempExcludeData;
            this.backupPerform = backupPerform;
            this.backupLocation = backupLocation;
            this.FKCheckSync = FKCheckSync;
        }
    }
}
