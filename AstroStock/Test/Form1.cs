using System;
using System.Windows.Forms;

public enum Title
{
    King,
    Sir
};

public class EnumsAndComboBox : Form
{
    private DataGridView dataGridView1 = new DataGridView();
    private BindingSource bindingSource1 = new BindingSource();

    public EnumsAndComboBox()
    {
        this.Load += new System.EventHandler(EnumsAndComboBox_Load);
    }

    private void EnumsAndComboBox_Load(object sender, System.EventArgs e)
    {
        // Populate the data source.
        bindingSource1.Add(new Knight(Title.King, "Uther", true, 40));
        bindingSource1.Add(new Knight(Title.King, "Arthur", true, 28));
        bindingSource1.Add(new Knight(Title.Sir, "Mordred", false, 37));
        bindingSource1.Add(new Knight(Title.Sir, "Gawain", true, 26));
        bindingSource1.Add(new Knight(Title.Sir, "Galahad", true, 41));

        // Initialize the DataGridView.
        dataGridView1.AutoGenerateColumns = false;
        dataGridView1.AutoSize = true;
        dataGridView1.DataSource = bindingSource1;

        dataGridView1.Columns.Add(CreateComboBoxWithEnums());

        // Initialize and add a text box column.
        DataGridViewColumn column = new DataGridViewTextBoxColumn();
        column.DataPropertyName = "Name";
        column.Name = "Knight";
        dataGridView1.Columns.Add(column);

        // Initialize and add a check box column.
        column = new DataGridViewCheckBoxColumn();
        column.DataPropertyName = "GoodGuy";
        column.Name = "Good";
        dataGridView1.Columns.Add(column);

        column = new DataGridViewTextBoxColumn();
        column.DataPropertyName = "Age";
        column.Name = "Age";
        dataGridView1.Columns.Add(column);

        // Initialize the form.
        this.Controls.Add(dataGridView1);
        this.AutoSize = true;
        this.Text = "DataGridView object binding demo";
    }

    DataGridViewComboBoxColumn CreateComboBoxWithEnums()
    {
        DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
        combo.DataSource = Enum.GetValues(typeof(Title));
        combo.DataPropertyName = "Title";
        combo.Name = "Title";
        return combo;
    }
    #region "business object"
    private class Knight
    {
        private string hisName;
        private bool good;
        private Title hisTitle;

        private int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public Knight(Title title, string name, bool good, int age)
        {
            hisTitle = title;
            hisName = name;
            this.good = good;
            this.age = age;
        }

        public Knight()
        {
            hisTitle = Title.Sir;
            hisName = "<enter name>";
            good = true;
            age = 30;
        }

        public string Name
        {
            get
            {
                return hisName;
            }

            set
            {
                hisName = value;
            }
        }

        public bool GoodGuy
        {
            get
            {
                return good;
            }
            set
            {
                good = value;
            }
        }

        public Title Title
        {
            get
            {
                return hisTitle;
            }
            set
            {
                hisTitle = value;
            }
        }
    }
    #endregion

    [STAThread]
    public static void Main()
    {
        Application.Run(new EnumsAndComboBox());
    }

}
