using Microsoft.EntityFrameworkCore;
using pract_15.Models;
using pract_15.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace pract_15
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // ===== ДАННЫЕ =====
        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Brand> Brands { get; set; }
        public List<TagItem> Tags { get; set; }

        public ICollectionView ProductsView { get; set; }

        // ===== ФИЛЬТРЫ =====
        public string SearchText { get; set; }
        public string _PriceFrom = "";
        public string _PriceTo = "";

        public Category SelectedCategory { get; set; }
        public Brand SelectedBrand { get; set; }
        public bool IsManager { get; set; } = false;

        private readonly ElectroShopDbContext db = DBService.Instance.Context;

        public string PriceFrom
        {
            get => _PriceFrom;
            set
            {
                if (_PriceFrom != value)
                {
                    _PriceFrom = value;
                    OnPropertyChanged();
                    ProductsView.Refresh();
                }
            }
        }

        public string PriceTo
        {
            get => _PriceTo;
            set
            {
                if (_PriceTo != value)
                {
                    _PriceTo = value;
                    OnPropertyChanged();
                    ProductsView.Refresh();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow(bool isManager)
        {
            IsManager = isManager;



            LoadProducts();

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = FilterProducts;

            Categories = new ObservableCollection<Category>(db.Categories.ToList());
            Brands = new ObservableCollection<Brand>(db.Brands.ToList());

            InitializeComponent();
            if (IsManager)
            {
                ManagerUI.Visibility = Visibility.Visible;
            }
        }
        public void LoadProducts()
        {
            Products.Clear();

            foreach (var p in db.Products
                .Include(x => x.Brand)
                .Include(x => x.Category)
                 .Include(p => p.Tags)

                )
            {
                Products.Add(p);
            }
        }



        private bool FilterProducts(object obj)
        {
            if (obj is not Product p) return false;

            if (!string.IsNullOrEmpty(SearchText) &&
                !p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;

            if (SelectedCategory != null && p.CategoryId != SelectedCategory.Id)
                return false;

            if (SelectedBrand != null && p.BrandId != SelectedBrand.Id)
                return false;
            if (!string.IsNullOrWhiteSpace(PriceFrom))
            {
                if (double.TryParse(PriceFrom, out double minPrice))
                {
                    if (Convert.ToDouble(p.Price) < minPrice)
                        return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(PriceTo))
            {
                if (double.TryParse(PriceTo, out double maxPrice))
                {
                    if (Convert.ToDouble(p.Price) > maxPrice)
                        return false;
                }
            }

            return true;
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            ProductsView.Refresh();
        }

      

        private void SortChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsView.SortDescriptions.Clear();

            if (((ComboBox)sender).SelectedItem is not ComboBoxItem item)
                return;

            var tag = item.Tag?.ToString();

            switch (tag)
            {
                case "PriceAsc":
                    ProductsView.SortDescriptions.Add(
                        new SortDescription("Price", ListSortDirection.Ascending));
                    break;

                case "PriceDesc":
                    ProductsView.SortDescriptions.Add(
                        new SortDescription("Price", ListSortDirection.Descending));
                    break;

                case "StockAsc":
                    ProductsView.SortDescriptions.Add(
                        new SortDescription("Stock", ListSortDirection.Ascending));
                    break;

                case "StockDesc":
                    ProductsView.SortDescriptions.Add(
                        new SortDescription("Stock", ListSortDirection.Descending));
                    break;

            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product();
            var window = new Windows.ProductEditWindow(product);

            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            var window = new Windows.ProductEditWindow(product);

            if (window.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (MessageBox.Show("Удалить товар?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            db.Entry(product)
              .Collection(p => p.Tags)
              .Load();
            product.Tags.Clear();
            db.SaveChanges();
            db.Products.Remove(product);
            db.SaveChanges();

            // и из UI
            Products.Remove(product);

            db.SaveChanges();

            Products.Remove(product);
        }
        private void Brands_Click(object sender, RoutedEventArgs e)
        {
            new Windows.BrandsWindow { Owner = this }.ShowDialog();
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            new Windows.CategoriesWindow { Owner = this }.ShowDialog();
        }

        private void Tags_Click(object sender, RoutedEventArgs e)
        {
            new Windows.TagsWindow { Owner = this }.ShowDialog();
        }

    }
}
