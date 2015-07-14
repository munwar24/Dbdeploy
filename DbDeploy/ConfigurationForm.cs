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
    public partial class ConfigurationForm : Form
    {
        public DbDeployForm dbDeployForm;
        public OdbcConnection meta;
        TableFilter previousFilter = null;

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void SchemaTablesEditButton_Click(object sender, EventArgs e)
        {
            //Configuration config = (Configuration)(dbDeployForm.configurationList.SelectedItem);
            TableSelectionsEdit(SchemaTableList, false); //, config.schemaTables );
        }

        private void TableSelectionsEdit(CheckedListBox listBox, bool filterList) //, Dictionary<string,TableSelect> dictionary)
        {
            TableSelectForm form = dbDeployForm.tableSelectForm;
            ListBox list = form.TableList;

            form.TablesLoad(meta);

            int index;

            foreach (TableSelect select in listBox.Items)
            {
                index = list.Items.IndexOf(select.name);
                if (index >= 0)
                {
                    list.SelectedItems.Add(select.name);
                }
                else
                {
                    //TODO: Issue an error that table no longer exists and will be immediately deleted (peform deletion here)
                }
            }

            if (dbDeployForm.tableSelectForm.ShowDialog() == DialogResult.OK)
            {
                bool found;

                foreach (string name in list.SelectedItems)
                {
                    found = false;
                    foreach (TableSelect select in listBox.Items)
                    {
                        if (name == select.name)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        TableSelect select;
                        if (filterList)
                            select = new TableFilter(true, name, "");
                        else
                            select = new TableSelect(true, name);
                        listBox.Items.Add(select);
                        listBox.SetItemChecked(listBox.Items.IndexOf(select), true);
                    }
                }

                index = 0;
                while (index < listBox.Items.Count)
                {
                    if (list.SelectedItems.Contains(((TableSelect)(listBox.Items[index])).name))
                    {
                        index++;
                    }
                    else
                    {
                        listBox.Items.RemoveAt(index);
                    }
                }

                foreach (TableSelect select in listBox.Items)
                {
                    if (!list.SelectedItems.Contains(select.name))
                    {
                        listBox.Items.Remove(select);
                    }
                }

            }
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            if (previousFilter != null)
                previousFilter.filter = FilterClauseText.Text;
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void DataTablesEditButton_Click(object sender, EventArgs e)
        {
            //            Configuration config = (Configuration)(dbDeployForm.configurationList.SelectedItem);
            TableSelectionsEdit(DataTableList, false); //, config.dataTables);
        }

        private void backupPerformCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (!backupPerformCheck.Checked)
            {
                if (MessageBox.Show("It is a requirement of the iiSDLC process to perform a backup prior to deploying a database.  By unchecking this you are stating that you have performed the backup using a different method.", "iiSDLC Process Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    backupPerformCheck.Checked = true;
                    MessageBox.Show("A backup must be performed prior to a deployment.  The perform backup check box remains checked.", "iiSDLC Process", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void FilterTablesEditButton_Click(object sender, EventArgs e)
        {
            TableSelectionsEdit(FilterTableList, true); //, config.dataTables);
        }

        private void FilterTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (previousFilter != null)
                previousFilter.filter = FilterClauseText.Text;

            previousFilter = (TableFilter)FilterTableList.SelectedItem;

            if (previousFilter == null)
            {
                FilterClauseText.Text = "";
                FilterClauseText.Enabled = false;
            }
            else
            {
                FilterClauseText.Text = previousFilter.filter;
                FilterClauseText.Enabled = true;
            }
        }
    }
}
