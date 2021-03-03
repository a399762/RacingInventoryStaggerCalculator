using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Stagger.Objects;
using Stagger.Dialog;
using Stagger.Report;
using System.Diagnostics;
using System.IO;

namespace Stagger
{
    public partial class Main : Form
    {
        List<DnumberSizeCount> DNumberSizes1 = new List<DnumberSizeCount>();
        List<DnumberSizeCount> DNumberSizes2 = new List<DnumberSizeCount>();

        Dnumber dNumber1 = new Dnumber();
        Dnumber dNumber2 = new Dnumber();
      
        private double defaultLow = 86;
        private double defaultHigh = 88;

        public Main()
        {
            InitializeComponent();
            reset();
            setupDnumberRanges();
        }

        private void reset()
        {
            comboBox1.SelectedIndex = 0;
            radioButton1.Checked = true;

            dnumber1TextBox.Text = "D1111";
            dnumber1HighTextBox.Text = "88.9";
            dnumber1LowTextBox.Text = "86.1";

            dnumber2TextBox.Text = "D2222";
            dnumber2HighTextBox.Text = "88.9";
            dnumber2LowTextBox.Text = "86.1";
        }

        private void OneDNumberStaggered(double Low, double High, double Goal, bool Optimized, int OpSetCount, int SetsL, int SetsR)
        {
            double BestOptimizedLow = Low;

            if (checkBox1.Checked)
            {
                while (Low < High)
                {
                    int TotalSets = GetTotalPossibleSets(DNumberSizes1, DNumberSizes1, Low, High, Goal, SetsL, SetsR);

                    if (TotalSets >= OpSetCount)
                    {
                        if (Math.Abs(Low - Goal) < Math.Abs(BestOptimizedLow - Goal))
                        {
                            BestOptimizedLow = Low;
                        }
                    }

                    resetUsed(DNumberSizes1);
            
                    Low = Math.Round(Low + 0.1, 1);
                }
            }

            List<Set> SetList = GetTotalSets(DNumberSizes1, DNumberSizes1, BestOptimizedLow, High, Goal,  SetsL,  SetsR);

            RefreshDataGrids(SetList);
        }

        private void TwoDnumberStaggerCalc(double Low, double High, double Goal, bool Optimized, int OpSetCount, int SetsL, int SetsR)
        {
            double BestOptimizedLow = Low;

            if (checkBox1.Checked)
            {
                while (Low < High)
                {
                    int TotalSets = GetTotalPossibleSets(DNumberSizes1, DNumberSizes2, Low, High, Goal,SetsL,SetsR);

                    if (TotalSets >= OpSetCount)
                    {
                        if (Math.Abs(Low - Goal) < Math.Abs(BestOptimizedLow - Goal))
                        {
                            BestOptimizedLow = Low;
                        }
                    }

                    resetUsed(DNumberSizes1);
                    resetUsed(DNumberSizes2);

                    Low = Math.Round(Low + 0.1, 1);
                } 
            }

            List<Set> SetList = GetTotalSets(DNumberSizes1, DNumberSizes2, BestOptimizedLow, High, Goal,  SetsL,  SetsR);

            RefreshDataGrids(SetList);
        }

        private int GetTotalPossibleSets(List<DnumberSizeCount> DNumberSizes1, List<DnumberSizeCount> DNumberSizes2, double Low, double High, double Goal, int SetsL, int SetsR)
        {
            List<Set> SetList = new List<Set>();
            KeyValuePair<Double, int> StaggerSet = FindOptimalStagger(DNumberSizes1, DNumberSizes2, Low, High, Goal, SetsL, SetsR);

            while (StaggerSet.Value > 0)
            {
                SetList.AddRange(GetUnusedSets(DNumberSizes1, DNumberSizes2, StaggerSet.Key, SetsL, SetsR));
                StaggerSet = FindOptimalStagger(DNumberSizes1, DNumberSizes2, Low, High, Goal, SetsL, SetsR);
            }

            int TotalSets = GetSetTotal(SetList);

            return TotalSets;
        }

        private List<Set> GetTotalSets(List<DnumberSizeCount> DNumberSizes1, List<DnumberSizeCount> DNumberSizes2, double Low, double High, double Goal, int SetsL, int SetsR)
        {
            List<Set> SetList = new List<Set>();
            KeyValuePair<Double, int> StaggerSet = FindOptimalStagger(DNumberSizes1, DNumberSizes2, Low, High, Goal, SetsL, SetsR);

            while (StaggerSet.Value > 0)
            {
                SetList.AddRange(GetUnusedSets(DNumberSizes1, DNumberSizes2, StaggerSet.Key, SetsL, SetsR));
                StaggerSet = FindOptimalStagger(DNumberSizes1, DNumberSizes2, Low, High, Goal, SetsL, SetsR);
            }

            return SetList;
        }

        private int GetSetTotal(List<Set> SetList)
        {
            int Count = 0;
            foreach (Set item in SetList)
            {
                Count += item.SetCount;
            }

            return Count;
        }

        #region Private Helper functions

        private List<Set> GetUnusedSets(List<DnumberSizeCount> DNumberSizes1, List<DnumberSizeCount> DNumberSizes2, double Stagger, int SetsL, int SetsR)
        {
            List<Set> TempSetList = new List<Set>();

            foreach (DnumberSizeCount DNumSize1 in DNumberSizes1)
            {
                foreach (DnumberSizeCount DNumSize2 in DNumberSizes2)
                {
                    double SizeDif = Math.Round(DNumSize2.Size - DNumSize1.Size, 1);
                    if (SizeDif == Stagger)
                    {
                        //find how many pairs their are of each of the selected size
                        int DN1Pairs = (int)((DNumSize1.Count - DNumSize1.Used) / SetsL);
                        int DN2Pairs = (int)((DNumSize2.Count - DNumSize2.Used) / SetsR);

                        if (DN1Pairs != 0 & DN2Pairs != 0)
                        {
                            //use the lowest pair value
                            int LowPair = (DN1Pairs <= DN2Pairs) ? DN1Pairs : DN2Pairs;

                            //use DN1Pairs value as the sets available, then mark them used 
                            DNumSize1.Used = DNumSize1.Used + LowPair * SetsL;
                            DNumSize2.Used = DNumSize2.Used + LowPair * SetsR;

                            Set NewSet = new Set();
                            NewSet.RightSize = DNumSize2.Size;
                            NewSet.LeftSize = DNumSize1.Size;

                            NewSet.SetCount = LowPair;
                            TempSetList.Add(NewSet);
                        }
                    }
                }
            }

            return TempSetList;
        }
       
        private void RefreshDataGrids(List<Set> Sets)
        {
            //update the datagrids to display used tires
            dataGridView3.DataSource = Sets;
            dataGridView3.Refresh();
            dataGridView2.Refresh();
            dataGridView1.Refresh();
        }

        private void resetUsed(List<DnumberSizeCount> DNumberSizes)
        {
            foreach (DnumberSizeCount DNumSize in DNumberSizes)
            {
                DNumSize.Used = 0;
            }
        }

        private void resetTempUsed(List<DnumberSizeCount> DNumberSizes)
        {
            foreach (DnumberSizeCount DNumSize in DNumberSizes)
            {
                DNumSize.TempUsed = DNumSize.Used;
            }
        }

        private KeyValuePair<Double, int> FindOptimalStagger(List<DnumberSizeCount> DNumberSizes1, List<DnumberSizeCount> DNumberSizes2, double Low, double High, double Goal,int SetsL, int SetsR)
        {
            double BestStagger = 0;
            int BestStaggerSetCount = 0;

            double CurrentStagger = Low;
            int currentStaggerSetCount = 0;

            while (CurrentStagger <= High)
            {
                resetTempUsed(DNumberSizes1);
                resetTempUsed(DNumberSizes2);
           
                foreach (DnumberSizeCount DNumSize1 in DNumberSizes1)
                {
                    foreach (DnumberSizeCount DNumSize2 in DNumberSizes2)
                    {
                        double SizeDif = Math.Round(DNumSize2.Size - DNumSize1.Size, 1);
                        if (SizeDif == CurrentStagger)
                        {
                            //find how many pairs their are of each of the selected size
                            int DN1Pairs = (int)((DNumSize1.Count - DNumSize1.TempUsed) / SetsL);
                            int DN2Pairs = (int)((DNumSize2.Count - DNumSize2.TempUsed) / SetsR);

                            if (DN1Pairs != 0 & DN2Pairs != 0)
                            {
                                //use the lowest pair value
                                int LowPair = (DN1Pairs <= DN2Pairs) ? DN1Pairs : DN2Pairs;

                                //use DN1Pairs value as the sets available, then mark them used 
                                DNumSize1.TempUsed = DNumSize1.TempUsed + LowPair * SetsL;
                                DNumSize2.TempUsed = DNumSize2.TempUsed + LowPair * SetsR;

                                currentStaggerSetCount = currentStaggerSetCount + LowPair;
                            }
                        }
                    }
                }
                if (BestStaggerSetCount <= currentStaggerSetCount)
                {
                    //find if this one is closer to Gaol than the other, if they are the same apart, ie, 0.9 , 1.1. use 0.9 
                    if (Math.Abs(CurrentStagger - Goal) < Math.Abs(BestStagger - Goal) | BestStaggerSetCount == 0)
                    {
                        BestStagger = CurrentStagger;
                        BestStaggerSetCount = currentStaggerSetCount;
                    }
                }

                //reset variables for next stagger check
                currentStaggerSetCount = 0;
                CurrentStagger = Math.Round(CurrentStagger + 0.1, 1);
            }

            KeyValuePair<Double, int> BestStaggerSet = new KeyValuePair<double, int>(BestStagger, BestStaggerSetCount);
            return BestStaggerSet;
        }

        private int GetCount(List<DnumberSizeCount> DNumberSizes)
        {
            int Count = 0;
            foreach (DnumberSizeCount item in DNumberSizes)
                Count = Count + item.Count;

            return Count;
        }
        #endregion

        #region Events
        private void calculateDnumber1Ranges()
        {
            dataGridView1.DataSource = null;
            DNumberSizes1.Clear();

            string Dnumber = dNumber1.DnumberKey;

            double Low = dNumber1.lowSize;
            double High = dNumber1.highSize;

            double next = Low;

            DNumberSizes1 = new List<DnumberSizeCount>();

            while (next <= High)
            {
                DnumberSizeCount tempSize = new DnumberSizeCount();
                tempSize.Size = next;
                tempSize.Count = 0;
                tempSize.Used = 0;

                DNumberSizes1.Add(tempSize);

                if (comboBox1.SelectedIndex == 0)
                {
                    next = Math.Round(next + .1, 1);
                }
                else
                {
                    next = Math.Round(next + .25, 2);
                }
            }

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = DNumberSizes1;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.LightBlue;
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[0].HeaderText = "Size: " + dNumber1.DnumberKey;
          
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Boolean ValidatedLow = false;
            Boolean ValidatedHigh = false;
            Boolean ValidatedGoal = false;
  
            Boolean ValidatedCars = false;
            Boolean ValidatedSetsL = false;
            Boolean ValidatedSetsR = false;
            
            double Low = 0;
            double High = 0;
            double Goal = 0;
          
            int Cars = 0;
            int SetsL = 0;
            int SetsR = 0;


            //Validate Stagger Range
            ValidatedLow = double.TryParse(textBox2.Text, out Low);
            ValidatedHigh = double.TryParse(textBox3.Text, out High);
            ValidatedGoal = double.TryParse(textBox6.Text, out Goal);
        
            ValidatedCars = int.TryParse(textBox5.Text, out Cars);
            ValidatedSetsL = int.TryParse(textBox8.Text, out SetsL);
            ValidatedSetsR = int.TryParse(textBox7.Text, out SetsR);

            if (ValidatedLow & ValidatedHigh & ValidatedGoal & ValidatedCars & ValidatedSetsL & ValidatedSetsR)
            {
                //end edit on the datagrid in case a user has not exited the control yet
                dataGridView1.EndEdit();
                dataGridView2.EndEdit();

                dataGridView3.DataSource = null;

                resetUsed(DNumberSizes1);
                resetUsed(DNumberSizes2);

                //Based on the selected Dnumber Setup, use the appropriate Algorithm to Calculate.
                if (radioButton1.Checked)
                {
                    TwoDnumberStaggerCalc(Low, High, Goal, checkBox1.Checked, Cars, SetsL, SetsR);
                }
                else if (radioButton3.Checked)
                {
                    OneDNumberStaggered(Low, High, Goal, checkBox1.Checked, Cars, SetsL, SetsR);
                }

                tabControl1.SelectedIndex = 1;
            }
            else 
            {
                MessageBox.Show("Check Parameters!");
            }
        }

        private void calculateDnumber2Ranges()
        {
            dataGridView2.DataSource = null;
            DNumberSizes2.Clear();

            string Dnumber = dNumber2.DnumberKey;

            double Low = dNumber2.lowSize;
            double High = dNumber2.highSize;

            double next = Low;

            DNumberSizes2 = new List<DnumberSizeCount>();

            while (next <= High)
            {
                DnumberSizeCount tempSize = new DnumberSizeCount();
                tempSize.Size = next;
                tempSize.Count = 0;
                tempSize.Used = 0;

                DNumberSizes2.Add(tempSize);

                if (comboBox1.SelectedIndex == 0)
                {
                    next = Math.Round(next + .1, 1);
                }
                else
                {
                    next = Math.Round(next + .25, 2);
                }
            }


            dataGridView2.DataSource = null;
            dataGridView2.DataSource = DNumberSizes2;
            dataGridView2.Columns[0].ReadOnly = true;
            dataGridView2.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView2.Columns[2].ReadOnly = true;
            dataGridView2.Columns[2].DefaultCellStyle.BackColor = Color.LightBlue;

            dataGridView2.Columns[3].Visible = false;
            dataGridView2.Columns[0].HeaderText = "Size: " + dNumber2.DnumberKey;
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                dnumber2TextBox.Enabled = true;
                dnumber2LowTextBox.Enabled = true;
                dnumber2HighTextBox.Enabled = true;

                dataGridView2.Enabled = true;
                dataGridView2.Visible = true;
            }
            else if (radioButton3.Checked)
            {
                dnumber2TextBox.Enabled = false;
                dnumber2LowTextBox.Enabled = false;
                dnumber2HighTextBox.Enabled = false;

                dataGridView2.Enabled = false;
                dataGridView2.Visible = false;
            }
            setupDnumberRanges();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 About = new AboutBox1();
            About.ShowDialog(this);
        }
        #endregion

        /// <summary>
        /// generate report button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string FileName = string.Format("StaggerCalculatorExport-{0:yyyy-MM-dd_hh-mm-ss-tt}.pdf", DateTime.Now);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Title = "Save Stagger Report";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.FileName = FileName;
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GenerateSummary SummaryExporter = new GenerateSummary();

                String D1name = dnumber1TextBox.Text;
                String D2name = dnumber2TextBox.Text;

                List<Set> TempSet = (List<Set>)dataGridView3.DataSource;

                if (radioButton1.Checked)
                {
                    SummaryExporter.Generate(saveFileDialog1.FileName, DNumberSizes1, DNumberSizes2, D1name, D2name, TempSet);
                }
                else
                {
                    SummaryExporter.GenerateSingle(saveFileDialog1.FileName, DNumberSizes1, D1name, TempSet);
                }

                try
                {
                    Process.Start(saveFileDialog1.FileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        public static void EnsurePathExists(String Path)
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
            }
            catch (Exception)
            {
            }
        }

        private void setupDnumberRanges()
        {
            dNumber1.DnumberKey = dnumber1TextBox.Text.ToUpper().Trim();
            dNumber2.DnumberKey = dnumber2TextBox.Text.ToUpper().Trim();

            double Low = defaultLow;
            double High = defaultHigh;
            double.TryParse(dnumber1LowTextBox.Text.Trim(), out Low);
            double.TryParse(dnumber1HighTextBox.Text.Trim(), out High);

            dNumber1.lowSize = Low;
            dNumber1.highSize = High;

            double Low2 = defaultLow;
            double High2 = defaultHigh;
            double.TryParse(dnumber2LowTextBox.Text.Trim(), out Low2);
            double.TryParse(dnumber2HighTextBox.Text.Trim(), out High2);


            dNumber2.lowSize = Low2;
            dNumber2.highSize = High2;


            calculateDnumber1Ranges();
            calculateDnumber2Ranges();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setupDnumberRanges();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}
