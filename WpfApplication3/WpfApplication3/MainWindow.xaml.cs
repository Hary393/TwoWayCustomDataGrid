using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();  //Logika Aplikacji 
        }

    }

    public class Person
    {
        public string Name { get; set; }

        public ObservableCollection<SmartPhone> SmartPhones { get; set; }
    }

    public class SmartPhone
    {
        public string Manufacturer { get; set; }

        public bool IsWorking { get; set; }
    }
    public class MyDataGrid : DataGrid
    {
        public ObservableCollection<string> ColumnHeaders
        {
            get { return GetValue(ColumnHeadersProperty) as ObservableCollection<string>; }
            set { SetValue(ColumnHeadersProperty, value); }
        }

        public static readonly DependencyProperty ColumnHeadersProperty = DependencyProperty.Register("ColumnHeaders", typeof(ObservableCollection<string>), typeof(MyDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnColumnsChanged)));

    static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = d as MyDataGrid;
            dataGrid.Columns.Clear();
            //Add Person Column
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "Name", Binding = new Binding("Name") });
            //Add Manufactures Columns
            foreach (var value in dataGrid.ColumnHeaders)
            {
                var column = new DataGridCheckBoxColumn() { Header = value, Binding = new Binding("SmartPhones") { ConverterParameter = value, Converter = new ManufacturerConverter() } };
               // var column2 = new DataGridTextColumn() { Header = value, Binding = new Binding("SmartPhones") { ConverterParameter = value, Converter = new ManufacturerConverter() } }; oryginalniezamiast check boxa będzie pole text z którego będę zczytywał inty 

                dataGrid.Columns.Add(column);
            }
        }
    }

    public class ManufacturerConverter : IValueConverter
    {
        public static ObservableCollection<SmartPhone> SmartPhonesRef { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var smartPhones = value as IEnumerable<SmartPhone>;
            SmartPhonesRef = (ObservableCollection<SmartPhone>)value;   ////////////dodaje odnośnik do source bo mi potrzebne w convert back
            if (smartPhones != null && parameter != null)
            {
                var phone = smartPhones.FirstOrDefault(s => s.Manufacturer == parameter.ToString());
                if (phone != null)
                    return phone.IsWorking;
                return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) ///////////////nie działa poprawnie ponieważ żeby zkonwertować z powrotem potrzebuje source którego nie mam
                                                                                        /////////// próbowałem to obejść dodając do pola klasy konwertera dodać odnośnik do kolekcji ale nie działa to poprawnie
                                                                                        ////////// dla wartości w prawym górnym rogu nie działa (po wyjściu z focusu dla wiersza nie updejtuje wartości najbardziej z prawej )
        {
            bool? x = value as bool?;
            bool newBool = x ?? false;
            var phone = SmartPhonesRef.FirstOrDefault(s => s.Manufacturer == parameter.ToString());
            if (phone != null)
           {
                int index = SmartPhonesRef.IndexOf(phone);
                phone.IsWorking = newBool;
                SmartPhonesRef[index] = phone;
            }
            return SmartPhonesRef;
        }
    }

    public class ViewModel
    {
        public ViewModel()
        {
            ColumnHeaders = new ObservableCollection<string>();
            PersonCollection = new ObservableCollection<Person>()
        {
            new Person(){Name="Foo",
                SmartPhones=new ObservableCollection<SmartPhone>()
                {new SmartPhone(){Manufacturer="Manufacturer1",IsWorking=true}
                    ,new SmartPhone(){Manufacturer="Manufacturer2",IsWorking=false}}}
        , new Person(){Name="Bar",
                SmartPhones=new ObservableCollection<SmartPhone>()
                {new SmartPhone(){Manufacturer="Manufacturer1",IsWorking=true}
                    ,new SmartPhone(){Manufacturer="Manufacturer2",IsWorking=false}
                    ,new SmartPhone(){Manufacturer="Manufacturer3",IsWorking=true}}}

        , new Person(){Name="FooBar",
                SmartPhones=new ObservableCollection<SmartPhone>()
                {new SmartPhone(){Manufacturer="Manufacturer1",IsWorking=true}
                    ,new SmartPhone(){Manufacturer="Manufacturer2",IsWorking=false}
                    ,new SmartPhone(){Manufacturer="Manufacturer3",IsWorking=true}
                    ,new SmartPhone(){Manufacturer="Manufacturer4",IsWorking=false}
                    ,new SmartPhone(){Manufacturer="Manufacturer5",IsWorking=true}
                }}


        };
            foreach (var item in PersonCollection.SelectMany(s => s.SmartPhones).Select(s => s.Manufacturer).Distinct())
            {
                ColumnHeaders.Add(item);
            }
        }

        public ObservableCollection<Person> PersonCollection { get; set; }

        public ObservableCollection<string> ColumnHeaders { get; set; }
    }
}
