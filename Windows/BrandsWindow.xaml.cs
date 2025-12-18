using pract_15.Models;
using pract_15.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace pract_15.Windows
{
    public partial class BrandsWindow : Window
    {
        private readonly ElectroShopDbContext db = DBService.Instance.Context;
        private ObservableCollection<Brand> items;

        public BrandsWindow()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            items = new ObservableCollection<Brand>(db.Brands.ToList());
            ItemsList.ItemsSource = items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new SimpleEditWindow("Бренд");
            if (win.ShowDialog() == true)
            {
                db.Brands.Add(new Brand { Name = win.Value });
                db.SaveChanges();
                Load();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is not Brand item)
            {
                MessageBox.Show("Выберите бренд");
                return;
            }

            var win = new SimpleEditWindow("Бренд", item.Name);
            if (win.ShowDialog() == true)
            {
                item.Name = win.Value;
                db.SaveChanges();
                Load();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is not Brand item)
            {
                MessageBox.Show("Выберите бренд");
                return;
            }

            bool hasProducts = db.Products
                .Any(p => p.BrandId == item.Id);

            if (hasProducts)
            {
                MessageBox.Show(
                    "Нельзя удалить бренд, так как он используется в товарах",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show(
                    "Удалить бренд?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            db.Brands.Remove(item);
            db.SaveChanges();

            Load();
        }



    }
}