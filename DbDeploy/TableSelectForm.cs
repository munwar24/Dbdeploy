using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;

namespace OmniDbDeploy
{
    public partial class TableSelectForm : Form
    {
        public bool tablesLoaded = false;

        public DbDeployForm dbDeployForm;

        private Logger log = Logger.Singleton;

        public TableSelectForm()
        {
            InitializeComponent();
        }

        public void TablesLoad(OdbcConnection connection)
        {
            TableList.SelectedItems.Clear();
            if (tablesLoaded)
                return;

            TableList.Items.Clear();

            connection.Open();

            DataTable data = connection.GetSchema("Tables");
            DataRow row;

            string catalog;
            string schema;
            string name;
            string type;
            string remarks;

            int dataIndex;

            log.progressSet(0, data.Rows.Count);

            for (dataIndex = 0; dataIndex < data.Rows.Count; dataIndex++)
            {
                row = data.Rows[dataIndex];

                catalog = row.ItemArray[0].ToString();
                schema = row.ItemArray[1].ToString();
                name = row.ItemArray[2].ToString();
                type = row.ItemArray[3].ToString();
                remarks = row.ItemArray[4].ToString();

                //log.statusUpdate(database.typeName + ": " + schema + "." + name);
                //log.statusUpdate(database.typeName + ": " + name);

                if (schema != "SYSIBM"
                    && schema != "SYSTOOLS"
                    && schema != "TOOLS_CATALOG"
                    && schema != "ASN"
                    && !name.StartsWith("EXPLAIN_")
                    && !name.StartsWith("ADVISE_")
                    && !name.StartsWith("IBMSNAP_")
                    )
                {
                    this.TableList.Items.Add(name); //new Table(database, catalog, schema, name, type, remarks));
                }

                log.progressUpdate(0, dataIndex);
            }
            log.progressHide(0);

            connection.Close();

            tablesLoaded = true;
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
