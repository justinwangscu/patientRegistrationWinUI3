using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using patientRegistration.Views.Panes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace patientRegistration
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
            mainFrame.Content = new ViewPatientsPage();
            App.MainAppFrame = mainFrame;
            //PatientListView.ItemsSource = Cats;

        }

        // List of cats
        private List<string> Cats = new List<string>()
{
            "Abyssinian",
            "Aegean",
            "American Bobtail",
            "Mutt",
        };


        //private async void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //myButton.Content = "Clicked";
        //    var welcomeDialog = new ContentDialog()
        //    {
        //        Title = "Hello from HelloWorld",
        //        Content = "Welcome to your first Windows App SDK app.",
        //        CloseButtonText = "Ok",
        //        XamlRoot = myButton.XamlRoot
        //    };
        //    await welcomeDialog.ShowAsync();
        //}


        //// Handle text change and present suitable items

        //private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Since selecting an item will also change the text,
        //    // only listen to changes caused by user entering text.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        var suitableItems = new List<string>();
        //        var splitText = sender.Text.ToLower().Split(" ");
        //        foreach (var cat in Cats)
        //        {
        //            var found = splitText.All((key) =>
        //            {
        //                return cat.ToLower().Contains(key);
        //            });
        //            if (found)
        //            {
        //                suitableItems.Add(cat);
        //            }
        //        }
        //        if (suitableItems.Count == 0)
        //        {
        //            suitableItems.Add("No results found");
        //        }
        //        sender.ItemsSource = suitableItems;
        //    }

        //}

        //// Handle user selecting an item, in our case just output the selected item.
        //private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    sender.Text = args.SelectedItem.ToString();
        //}

        //private void OnAddPatientClick(object sender, RoutedEventArgs e)
        //{
        //    PatientListView.ItemsSource = Cats;
        //}

        


    }
}
