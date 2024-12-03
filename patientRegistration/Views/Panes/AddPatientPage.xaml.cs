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
using Windows.ApplicationModel.SocialInfo;
using Windows.Networking.NetworkOperators;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace patientRegistration.Views.Panes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPatientPage : Page
    {
        public AddPatientPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var pat = e.Parameter as Patient;
            if(pat != null)
            {
                System.Diagnostics.Debug.WriteLine($"AddPatientPage in Update Mode!");
                updateMode = true;
                loadPatient(pat);
            }
        }

        private bool updateMode = false;
        private ObservableCollection<Contact> newContactList { get; set; } = new ObservableCollection<Contact>();

        private Patient newPatient { get; set; } = new Patient();
        private Patient? oldPatient { get; set; } = null;

        public void loadPatient(Patient pat)
        {

            if(pat == null)
            {
                return;
            }

            oldPatient = pat;

            nameBox.Text = pat.Name;
            medRecBox.Text = pat.MedicalRecordNumber;
            ageBox.Text = pat.Age.ToString();
            genderBox.Text = pat.Gender;
            diagnosisBox.Text = pat.AdmittingDiagnosis;

            System.Diagnostics.Debug.WriteLine($"Patient DB ID: {oldPatient.DBID}");

            if(pat.Contacts != null)
            {
                foreach (Contact c in pat.Contacts)
                {
                    newContactList.Add(c);
                }
            }

        }

        private void addContactFromTextBox()
        {
            // Get fields from text boxes
            string name = contactNameBox.Text;
            string relationship = contactRelationshipBox.Text;
            string contactInfo = contactInfoBox.Text;
            

            // Contact Info is required
            if (contactInfo == "")
            {
                contactInfoBox.PlaceholderText = "Required";
                return;

            }

            // Set textboxes to empty string
            contactNameBox.Text = "";
            contactRelationshipBox.Text = "";
            contactInfoBox.Text = "";

            // create new contact and add to observable list
            Contact newContact = new(name, relationship, contactInfo);
            if(oldPatient != null)
            {
                newContact.PatientDBID = oldPatient.DBID;    
            }
            newContactList.Add(newContact);
        }
        
        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get fields from text boxes
            string name = nameBox.Text;
            string medRecNum = medRecBox.Text;
            string stringAge = ageBox.Text;
            string gender = genderBox.Text;
            string diagnosis = diagnosisBox.Text;

            int age = -1;

            // Name is required
            if (name == "")
            {
                nameBox.PlaceholderText = "Required";
                return;
            }

            if(!DataAccess.isUniqueMedRecNum(medRecNum))
            {

            }

            // If age is not empty string -> get int
            if (stringAge != "" && stringAge != null)
            {
                try
                {
                    age = Int32.Parse(stringAge);
                
                }
                catch (FormatException)
                {
                    ageBox.Text = "";
                    ageBox.PlaceholderText = "Age must be an integer";
                    return;
                }
            }

            addContactFromTextBox();

            newPatient.Name = name;
            newPatient.MedicalRecordNumber = medRecNum;
            newPatient.Gender = gender;
            newPatient.Age = age;
            newPatient.Gender = gender;
            newPatient.addDiagnosis(diagnosis);

            newPatient.addContacts(newContactList);

            if (updateMode && oldPatient != null)
            {
                newPatient.DBID = oldPatient.DBID;
                PatientsOpe.UpdatePatient(newPatient, oldPatient);
            }
            else
            {
                PatientsOpe.AddPatient(newPatient);
            }

            App.MainAppFrame.Navigate(typeof(ViewPatientsPage));

        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            App.MainAppFrame.Navigate(typeof(ViewPatientsPage));
        }

        private void addContactBtn_Click(object sender, RoutedEventArgs e)
        {
            addContactFromTextBox();
        }



        private void deleteFlyout_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var contact = item as Contact;

            if(contact == null)
            {
                return;
            }

            newContactList.Remove(contact);

        }

        private void editFlyout_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            var contact = item as Contact;

            if (contact == null)
            {
                return;
            }

            newContactList.Remove(contact);

            contactNameBox.Text = contact.Name;
            contactRelationshipBox.Text = contact.Relationship;
            contactInfoBox.Text = contact.ContactInfo;

        }
    }
}
