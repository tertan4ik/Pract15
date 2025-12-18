using Microsoft.EntityFrameworkCore;
using pract_15.Models;
using pract_15.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace pract_15
{
    public partial class MainWindow : Window
    {
        // ===== ƒ¿ÕÕ€≈ =====
        public ObservableCollection<Product> Products { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Brand> Brands { get; set; }
        public List<TagItem> Tags { get; set; }

        public ICollectionView ProductsView { get; set; }

        // ===== ‘»À‹“–€ =====
        public string SearchText { get; set; }
        public string PriceFrom { get; set; }
        public string PriceTo { get; set; }

        public Category SelectedCategory { get; set; }
        public Brand SelectedBrand { get; set; }
        public bool IsManager { get; set; } = false;
        // ===== ¡ƒ =====
        private readonly ElectroShopDbContext db = DBService.Instance.Context;

        // =====================================================================
        //  ŒÕ—“–” “Œ–
        // =====================================================================
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

        // =====================================================================
        // «¿√–”« ¿ “Œ¬¿–Œ¬
        // =====================================================================
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

        // =====================================================================
        // ‘»À‹“–¿÷»ﬂ
        // =====================================================================
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

            if (!string.IsNullOrEmpty(PriceFrom) && p.Price < Convert.ToDecimal(PriceFrom))
                return false;

            if (!string.IsNullOrEmpty(PriceTo) && p.Price > Convert.ToDecimal(PriceTo))
                return false;

            return true;
        }

        private void FiltersChanged(object sender, EventArgs e)
        {
            ProductsView.Refresh();
        }

        // =====================================================================
        // —Œ–“»–Œ¬ ¿
        // =====================================================================
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
                MessageBox.Show("¬˚·ÂËÚÂ ÚÓ‚‡");
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
                MessageBox.Show("¬˚·ÂËÚÂ ÚÓ‚‡");
                return;
            }

            if (MessageBox.Show("”‰‡ÎËÚ¸ ÚÓ‚‡?",
                "œÓ‰Ú‚ÂÊ‰ÂÌËÂ",
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

            // Ë ËÁ UI
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
