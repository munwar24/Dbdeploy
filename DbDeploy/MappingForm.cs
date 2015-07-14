using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OmniDbDeploy
{
    public partial class MappingForm : Form
    {
        public MappingForm()
        {
            InitializeComponent();
        }

        private void newColumnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            map();
        }

        private void oldColumnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            map();
        }


        private void map()
        {
            if (oldColumnList.SelectedItem != null && newColumnList.SelectedItem != null)
            {
                mappedColumnList.Items.Add(new Mapping(((Column)oldColumnList.SelectedItem), ((Column)newColumnList.SelectedItem)));
                oldColumnList.Items.Remove(oldColumnList.SelectedItem);
                newColumnList.Items.Remove(newColumnList.SelectedItem);
            }
        }

        private void UnmapButton_Click(object sender, EventArgs e)
        {
            Mapping map = ((Mapping)mappedColumnList.SelectedItem);
            oldColumnList.Items.Add(map.fromColumn);
            newColumnList.Items.Add(map.toColumn);
            mappedColumnList.Items.Remove(map);
        }

        private void mappedColumnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mappedColumnList.SelectedItem == null)
                UnmapButton.Enabled = false;
            else
                UnmapButton.Enabled = true;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
