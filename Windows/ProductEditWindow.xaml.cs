using pract_15.Models;
using pract_15.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pract_15.Windows
{
    public partial class ProductEditWindow : Window
    {
        public Product Product { get; }
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Brand> Brands { get; }
        public List<TagItem> Tags { get; set; }
        public Category SelectedCategory { get; set; }
        public Brand SelectedBrand { get; set; }

        private readonly ElectroShopDbContext db = DBService.Instance.Context;

        public ProductEditWindow(Product product)
        {
            Product = product;

            Categories = new ObservableCollection<Category>(db.Categories.ToList());
            Brands = new ObservableCollection<Brand>(db.Brands.ToList());

            SelectedCategory = Categories.FirstOrDefault(x => x.Id == Product.CategoryId);
            SelectedBrand = Brands.FirstOrDefault(x => x.Id == Product.BrandId);
            var selectedTagIds = Product.Tags.Select(t => t.Id).ToList();

            Tags = db.Tags
                .Select(t => new TagItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsSelected = selectedTagIds.Contains(t.Id)
                })
                .ToList();

            DataContext = this;


            DataContext = this;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(this))
            {
                MessageBox.Show("Исправьте ошибки ввода");
                return;
            }

            Product.CategoryId = SelectedCategory.Id;
            Product.BrandId = SelectedBrand.Id;


         if (Product.Id == 0)
            {
                db.Products.Add(Product);
                db.SaveChanges();
            }
  
            Product.Tags.Clear();

            foreach (var tag in Tags.Where(t => t.IsSelected))
            {
                var tagEntity = db.Tags.First(t => t.Id == tag.Id);
                Product.Tags.Add(tagEntity);
            }

            // сохраняем изменения
            db.SaveChanges();

            DialogResult = true;
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
