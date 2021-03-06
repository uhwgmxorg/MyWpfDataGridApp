using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace MyWpfDataGridApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return clickCommand ?? (clickCommand = new CommandHandler(() => MyAction(), () => CanExecute));
            }
        }
        public bool CanExecute
        {
            get
            {
                return true;
            }
        }

        #region INotify Changed Properties  
        private string message;
        public string Message
        {
            get { return message; }
            set { SetField(ref message, value, nameof(Message)); }
        }

        private ObservableCollection<Address> addresses;
        public ObservableCollection<Address> Addresses
        {
            get { return addresses; }
            set { SetField(ref addresses, value, nameof(Addresses)); }
        }
        private Address selectedAddress;
        public Address SelectedAddress
        {
            get { return selectedAddress; }
            set { SetField(ref selectedAddress, value, nameof(SelectedAddress)); }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

#if DEBUG
            Title += "    Debug Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#else
            Title += "    Release Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
#endif
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// MyAction
        /// </summary>
        public void MyAction()
        {
            System.Diagnostics.Debug.WriteLine("MyAction");
            System.Diagnostics.Debug.WriteLine($"SelectedAddress.Id {SelectedAddress.Id}");
            Message = $"You pressed the Button in the row with Id {SelectedAddress.Id} IsValis is {SelectedAddress.IsValid}";
        }

        /// <summary>
        /// Button_1_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_1_Click(object sender, RoutedEventArgs e)
        {
            Message = "You pressed Debug and Test Button #1";
            foreach (var a in Addresses)
                a.IsValid = false;
        }

        /// <summary>
        /// Button_Load_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            Message = "Load Data";
            Addresses = LToO<Address>(LoadList<Address>("Data.xml"));
        }

        /// <summary>
        /// Button_Save_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Message = "Save Data";
            SaveList<Address>(OToL<Address>(Addresses), "Data.xml");
        }

        /// <summary>
        /// Button_Clear_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            Message = "Clear Data";
            Addresses.Clear();
        }

        /// <summary>
        /// Button_Default_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Default_Click(object sender, RoutedEventArgs e)
        {
            Message = "Load Default";
            Addresses.Clear();
            LoadDefaultData();
        }

        /// <summary>
        /// Button_Close_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDefaultData();
        }

        /// <summary>
        /// Lable_Message_MouseDown
        /// Clear Message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lable_Message_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Message = "";
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// LoadDefaultData
        /// Load the data in the ObservableCollection Addresses Property
        /// </summary>
        private void LoadDefaultData()
        {
            int count = 1;
            System.Collections.ObjectModel.ObservableCollection<Address> addresses = new System.Collections.ObjectModel.ObservableCollection<Address>();

            #region Address data
            var dataAddress = new[] {
                new { Name = "James Mitchell", IsValid = true,Job = "Becker", City = "Bosten", Street = "Baker Street 3", Telefon = "(845)-523-6824"},
                new { Name = "Roscoe Crona", IsValid = true, Job = "Plumber", City = "Los Angeles", Street = "Well Street 67", Telefon = "(238)-475-0563"},
                new { Name = "Arvid O'Connell", IsValid = true, Job = "Miller", City = "Chicago", Street = "Hill Avenue 1", Telefon = "(458)-857-7797"},
                new { Name = "Martina Mayert", IsValid = true, Job = "Teacher", City = "Phoenix", Street = "Midd Street 5", Telefon = "(735)-630-1487"},
                new { Name = "Rick Hills", IsValid = true, Job = "Miller", City = "Philadelphia", Street = "Highway 1", Telefon = "(443)-706-4144"},
                new { Name = "Zachary Doyle", IsValid = true, Job = "Teacher", City = "Dallas", Street = "Wellington Street 3", Telefon = "(506)-588-710"},
            };

            foreach (var n in dataAddress)
                addresses.Add(new Address { Id = count++, Name = n.Name, IsValid = n.IsValid, Job = n.Job, City = n.City, Street = n.Street, Telefon = n.Telefon });
            #endregion

            Addresses = addresses;
        }

        /// <summary>
        /// LoadList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private System.Collections.Generic.List<T> LoadList<T>(string file)
        {
            System.Collections.Generic.List<T> list = null;

            try
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(System.Collections.Generic.List<T>));
                //XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(List<T>) })[0];

                using (System.IO.StreamReader rd = new System.IO.StreamReader(file))
                {
                    list = xs.Deserialize(rd) as System.Collections.Generic.List<T>;
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                list = new System.Collections.Generic.List<T>();
            }
            return list;
        }

        /// <summary>
        /// SaveList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        private void SaveList<T>(System.Collections.Generic.List<T> list, string file)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(System.Collections.Generic.List<T>));
                //XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(List<T>) })[0];
                using (System.IO.StreamWriter wr = new System.IO.StreamWriter(file))
                {
                    xs.Serialize(wr, list);
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// LToO
        /// converting a List to ObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="llist"></param>
        /// <returns></returns>
        private System.Collections.ObjectModel.ObservableCollection<T> LToO<T>(System.Collections.Generic.List<T> llist)
        {
            var oc = new System.Collections.ObjectModel.ObservableCollection<T>();
            foreach (var item in llist)
                oc.Add(item);
            return oc;
        }

        /// <summary>
        /// OToL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="olist"></param>
        /// <returns></returns>
        static public System.Collections.Generic.List<T> OToL<T>(System.Collections.ObjectModel.ObservableCollection<T> olist)
        {
            System.Collections.Generic.List<T> l = new System.Collections.Generic.List<T>(olist);
            return l;
        }

        /// <summary>
        /// SetField
        /// for INotify Changed Properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        #endregion
    }

    public class Address : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        public int Id
        {
            get { return id; }
            set { SetField(ref id, value, nameof(Id)); }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value, nameof(Name)); }
        }
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set { SetField(ref isValid, value, nameof(IsValid)); }
        }
        private string job;
        public string Job
        {
            get { return job; }
            set { SetField(ref job, value, nameof(Job)); }
        }
        private string city;
        public string City
        {
            get { return city; }
            set { SetField(ref city, value, nameof(City)); }
        }
        private string street;
        public string Street
        {
            get { return street; }
            set { SetField(ref street, value, nameof(Street)); }
        }
        private string telefon;
        public string Telefon
        {
            get { return telefon; }
            set { SetField(ref telefon, value, nameof(Telefon)); }
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }

    public class JobList : List<string>
    {
        public JobList()
        {
            Add("Becker");
            Add("Plumber");
            Add("Miller");
            Add("Teacher");
        }
    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
