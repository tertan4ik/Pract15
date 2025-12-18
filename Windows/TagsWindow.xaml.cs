using pract_15.Models;
using pract_15.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace pract_15.Windows
{
    public partial class TagsWindow : Window
    {
        private readonly ElectroShopDbContext db = DBService.Instance.Context;
        private ObservableCollection<Tag> items;

        public TagsWindow()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            items = new ObservableCollection<Tag>(db.Tags.ToList());
            ItemsList.ItemsSource = items;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new SimpleEditWindow("Тег");
            if (win.ShowDialog() == true)
            {
                db.Tags.Add(new Tag { Name = win.Value });
                db.SaveChanges();
                Load();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is not Tag item)
            {
                MessageBox.Show("Выберите тег");
                return;
            }

            var win = new SimpleEditWindow("Тег", item.Name);
            if (win.ShowDialog() == true)
            {
                item.Name = win.Value;
                db.SaveChanges();
                Load();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is not Tag item)
            {
                MessageBox.Show("Выберите тег");
                return;
            }

            if (MessageBox.Show(
                    "Удалить тег?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            // ОБЯЗАТЕЛЬНО подгружаем связанные товары
            db.Entry(item)
              .Collection(t => t.Products)
              .Load();

            // разрываем связи many-to-many
            item.Products.Clear();
            db.SaveChanges();

            // теперь можно удалять сам тег
            db.Tags.Remove(item);
            db.SaveChanges();

            Load();
        }

    }
}