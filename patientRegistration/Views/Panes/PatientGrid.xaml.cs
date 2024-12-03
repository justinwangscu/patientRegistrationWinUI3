using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;
using Windows.UI.Popups;
using patientRegistration;
using Windows.UI.WebUI;
using Windows.Devices.Enumeration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace patientRegistration.Views.Panes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientGrid : Page
    {

        public PatientGrid()
        {
            this.InitializeComponent();
        }

        public void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "MedicalRecordNumber":
                    e.Column.Header = "Medical Record Number";
                    break;

                case "AdmittingDiagnosis":
                    e.Column.Header = "Admitting Diagnosis";
                    break;

                case "AttendingPhysician":
                    e.Column.Header = "Attending Physician";
                    break;

                case "Contacts":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;

                case "DBID":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;

                case "CanBeDeleted":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                    
                default:
                    break;
            }
            e.Column.IsReadOnly = true;
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Patient pat = (Patient)dataGrid.SelectedItem;
            if(pat.AdmittingDiagnosis != "")
            {
                return;
            }
            dataGrid.SelectedIndex = -1;

            //DataAccess.remove(pat)

            PatientsOpe.DeletePatient(pat);
            //var result = Flyout.ShowAttachedFlyout();
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            Patient pat = (Patient)dataGrid.SelectedItem;
            App.MainAppFrame.Navigate(typeof(AddPatientPage), pat);
        }

        
    };

}
