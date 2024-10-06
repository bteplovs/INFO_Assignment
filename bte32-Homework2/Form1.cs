using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace bte32_Homework2
{
    public partial class Form1 : Form
    {
        
        private static Dictionary<string, int> CRUISE_DICTIONARY = new Dictionary<string, int>
        {
            { "Aurora", 13 },
            { "Arcadia", 15 },
            { "Adonia", 16 },
            { "Oceana", 17 },
            { "Grand Princess", 18 },
            { "Oriana", 20 },
            { "Dawn Princess", 23 }
        };
            
        int cruiseValue;
        string[] Data;
        List<string> shipData;
        List<string> cruiseData;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadCruiseFile();
        }
        private void cbxShipName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected();
            CruisesOfSelectedShip(); 
            LoadCruises();
            // essential polishing so that when a ship is changed the grid, cruise, and counters set back to default
            dataGridViewCruiseInfo.Rows.Clear();
            cbxCruisesFound.Text = "";
            lblTotalSelected.Text = "0";
        }

        private void cbxCruisesFound_SelectedIndexChanged(object sender, EventArgs e)
        {
            InfoOfSelectedCruise();
            LoadInfo();
        }

        private void dataGridViewCruiseInfo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Click on one of the available cruises for a message box prompt to confirm the selection
            // call the datagrid object with forms grid and use the forms current state to fill a message box with the selected data
            if (e.RowIndex >= 0) { 
            
            DataGridViewRow selectedRow = dataGridViewCruiseInfo.Rows[e.RowIndex];

                string ship = cbxShipName.Text;
                string cruise = cbxCruisesFound.Text;
                string date = selectedRow.Cells[0].Value.ToString();
                string duration = selectedRow.Cells[1].Value.ToString();
                string cost = selectedRow.Cells[2].Value.ToString();

                MessageBox.Show($"You selected the cruise: {cruise}\n" +
                    $"Ship: {ship}\n" +
                    $"Date: {date}\n" +
                    $"Duration: {duration}\n" +
                    $"Cost: {cost}\n" +
                    "Would you like to confirm your selection?",
                    "Cruise Selection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
            }
            else
            {
                MessageBox.Show("Invalid Selection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// 
        ///  HELPER METHODS
        /// 


        private void ReadCruiseFile()
        {
            // loads the .txt
            Data = File.ReadAllLines("Cruise.txt");
            // .ReadAllLines because the text file is small
        }

        private void Selected()
        {   // using key:val -> shipName:shipNum, finds the ships number. helps traverse the data
            string selectedCruise = cbxShipName.Text;
            if (CRUISE_DICTIONARY.TryGetValue(selectedCruise, out int newCruiseValue))
            {
                cruiseValue = newCruiseValue;
            }
        }

        private void CruisesOfSelectedShip()
        {
            // once a ship is selected initilise new list with only cruises offered by that ship
            // this is done by appending the data items that share the ship num in index[1]
            List<string> newShipData = new List<string>();
            // using a list over array allows for easier appending
            foreach (string line in Data)
            {
                string[] parts = line.Split(',');

                if (parts[1].Trim() == cruiseValue.ToString())
                {
                    newShipData.Add(line);
                }
            }
            shipData = newShipData;
        }

        private void LoadCruises()
        {
            // Loads each unique cruise into the combobox and tracks a count of each cruise added so the user knows how many unique cruises are available
            cbxCruisesFound.Items.Clear();
            int count = 0;
            foreach (string line in shipData)
            {
                string[] parts = line.Split(',');

                // if a cruise is not in the list add to cbx, else, add nothing
                if (!cbxCruisesFound.Items.Contains(parts[5]))
                { 
                    cbxCruisesFound.Items.Add(parts[5]);
                    count++;
                }
            }
            lblCounter.Text = count.ToString();
            // for each cruise a ship offers, add it to the combo box for user to select without duplicates and count the
            // number of cruises available
        }

        private void InfoOfSelectedCruise()
        {
            // once a cruise is selected initilise new list with corresponding cruise dates/times/cost
            // When a cruise type is selected, dates/times/costs are rewritten to cruise data as to refresh with each selection
            List<string> newCruiseData = new List<string>();
            // using a list over array allows for easier appending
            foreach (string line in shipData)
            {
                string[] parts = line.Split(',');

                if (parts[5].Trim() == cbxCruisesFound.Text)
                {
                    newCruiseData.Add(line);
                }
            }
            cruiseData = newCruiseData;
            // cruiseData holds a list of a selected ship -> selected cruise 
        }

        private void LoadInfo()
        {
            // loads info into the datagridview by adding corresponding list index to column
            // using the current instance of cruisedata, display dates/times/costs 
            // from a selected ship --> selected cruise
            dataGridViewCruiseInfo.Rows.Clear();
            int count = 0;
            foreach (string line in cruiseData)
            {
                string[] parts = line.Split(',');

                string startDate = parts[4];
                string duration = parts[2];
                string costString = parts[3];
                    
                if (decimal.TryParse(costString, out decimal cost))
                {
                    dataGridViewCruiseInfo.Rows.Add(
                    DateTime.Parse(startDate).ToString("MMMM dd, yyyy"),
                    (duration + " days"),
                    cost.ToString("C")
                    );
                    count++;
                    // data is loaded by row, each iteration through a cruiseData adds start_date/duration/cost to the datagrid
                }
            }
            lblTotalSelected.Text = count.ToString();
        }
        private void Clear()
        {
            // Clears all fields
            cbxShipName.Text = "";
            cbxCruisesFound.Items.Clear();
            cbxCruisesFound.Text = "";
            lblCounter.Text = "0";
            lblTotalSelected.Text = "0";
            dataGridViewCruiseInfo.Rows.Clear();
        }
    }
}



