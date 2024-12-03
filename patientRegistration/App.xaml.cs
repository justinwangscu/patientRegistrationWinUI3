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
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.WebUI;
using Windows.ApplicationModel.Contacts.Provider;
using System.Xml.Linq;






// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace patientRegistration
{

    using Path = System.IO.Path;

    public record Contact
    {
        public string Name { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public int PatientDBID { get; set; } = -1;

        public int DBID { get; set; } = -1;

        public Contact()
        {

        }

        // Use constructor with no PatientDBID arg to make a Contact when it is currently unknown
        public Contact(string name, string contactInfo)
        {
            Name = name;
            ContactInfo = contactInfo;
        }
        public Contact(string name, string relationship, string contactInfo)
        {
            Name = name;
            Relationship = relationship;
            ContactInfo = contactInfo;
        }


        public Contact(string name, string relationship, string contactInfo, int patientDBID)
        {
            Name = name;
            Relationship = relationship;
            ContactInfo = contactInfo;
            PatientDBID = patientDBID;
        }

        public Contact(string name, string relationship, string contactInfo, int patientDBID, int dbid)
        {
            Name = name;
            Relationship = relationship;
            ContactInfo = contactInfo;
            PatientDBID = patientDBID;
            DBID = dbid;
        }

    }
    public record Patient
    {
        public string Name { get; set; } = string.Empty;
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public int Age { get; set; } = -1;
        public string Gender { get; set; } = string.Empty;
        public string AdmittingDiagnosis { get; set; } = string.Empty;
        public string AttendingPhysician { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int DBID { get; set; } = -1;
        public List<Contact> Contacts { get; set;} = new List<Contact>();

        public bool CanBeDeleted => string.IsNullOrEmpty(AdmittingDiagnosis);

        public Patient()
        {
        }
        // when a patient is read from the database use this constructor
        public Patient(string name, string medicalRecordNumber, int age, string gender, string admittingDiagnosis, string attendingPhysician, string department, int dbid)
        {
            Name = name;
            MedicalRecordNumber = medicalRecordNumber;
            Age = age;
            Gender = gender;
            AdmittingDiagnosis = admittingDiagnosis;
            AttendingPhysician = attendingPhysician;
            Department = department;
            DBID = dbid;

            Contacts = new List<Contact>();

        }

        // when patient information is added or edited with an admittingDiagnosis this constructor
        public Patient(string name, string medicalRecordNumber, int age, string gender, string admittingDiagnosis)
        {
            Name = name;
            MedicalRecordNumber = medicalRecordNumber;
            Age = age;
            Gender = gender;

            addDiagnosis(admittingDiagnosis);

            Contacts = new List<Contact>();


            Contacts.Add(new Contact("Yoko", "Mother", "(408) 777-9999", 1));
            Contacts.Add(new Contact("Yoko", "Mother", "(408) 777-9999", 1));
            Contacts.Add(new Contact("Yoko", "Mother", "(408) 777-9999", 2));
            Contacts.Add(new Contact("Yoko", "Mother", "(408) 777-9999", 2));
        }

        public Patient(string name, string medicalRecordNumber, int age, string gender)
        {
            Name = name;
            MedicalRecordNumber = medicalRecordNumber;
            Age = age;
            Gender = gender;
            Contacts = new List<Contact>();
        }
        public Patient(string name, string medicalRecordNumber, int age)
        {
            Name = name;
            MedicalRecordNumber = medicalRecordNumber;
            Age = age;
            Contacts = new List<Contact>();
        }
        public Patient(string name, string medicalRecordNumber)
        {
            Name = name;
            MedicalRecordNumber = medicalRecordNumber;
            Contacts = new List<Contact>();
        }
        public Patient(string name)
        {
            Name = name;
            Contacts = new List<Contact>();
        }

        public Patient(Patient pat)
        {
            if(pat == null)
            {
                return;
            }
            Name = pat.Name;
            MedicalRecordNumber = pat.MedicalRecordNumber;
            Age = pat.Age;
            Gender = pat.Gender;
            AdmittingDiagnosis = pat.AdmittingDiagnosis;
            AttendingPhysician = pat.AttendingPhysician;
            Department = pat.Department;
            DBID = pat.DBID;

            Contacts = new List<Contact>();

            if (pat.Contacts != null)
            {
                foreach (Contact c in pat.Contacts)
                {
                    Contacts.Add(c);
                }

            }

            setContactsFromDataBase();
        }

        public void setContactsFromDataBase()
        {
            // read from Database
            if (DBID == -1)
            {
                System.Diagnostics.Debug.WriteLine($"setContactsFromDataBase() Error: DBID is null");
            }


            // Add contacts to Contacs
            Contacts = DataAccess.GetPatientContacts(DBID);

        }

        public void addContact(Contact c)
        {
            Contacts.Add(c);
        }

        public void addContacts(IEnumerable<Contact> cList)
        {
            foreach (Contact c in cList)
            {
                Contacts.Add(c);
            }
        }

        public void addDiagnosis(string d)
        {
            AdmittingDiagnosis = d;

            string lowerCaseDiagnosis = AdmittingDiagnosis.ToLower();

            if (lowerCaseDiagnosis == "")
            {
                AttendingPhysician = "";
                Department = "";
            }
            else if ((lowerCaseDiagnosis.Contains("breast") || lowerCaseDiagnosis.Contains("lung")) && lowerCaseDiagnosis.Contains("cancer"))
            {
                AttendingPhysician = "Susan Jones";
                Department = "J";
            }
            else
            {
                AttendingPhysician = "Ben Smith";
                Department = "S";
            }

        }
    }

    public static class DataAccess
    {
        public static string dbpath { get; set; } = Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                         "patients.db");
        public async static void InitializeDatabase()
        {
            System.Diagnostics.Debug.WriteLine($"Local Path: {ApplicationData.Current.LocalFolder.Path}");
            
            await ApplicationData.Current.LocalFolder
                    .CreateFileAsync("patients.db", CreationCollisionOption.OpenIfExists);

            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                string createPatientsTable = @"
                    CREATE TABLE IF NOT EXISTS Patients (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        MedicalRecordNumber TEXT NOT NULL UNIQUE,
                        Age INTEGER NOT NULL,
                        Gender TEXT NOT NULL,
                        AdmittingDiagnosis TEXT NOT NULL,
                        AttendingPhysician TEXT NOT NULL,
                        Department TEXT NOT NULL
                    );";
                string createContactsTable = @"
                    CREATE TABLE IF NOT EXISTS Contacts (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        PatientID INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Relationship TEXT NOT NULL,
                        ContactInfo TEXT NOT NULL,
                        FOREIGN KEY (PatientID) REFERENCES Patients (ID) ON DELETE CASCADE ON UPDATE CASCADE
                    );
                ";

                using (var command = new SqliteCommand(createPatientsTable, db))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createContactsTable, db))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void GetAllPatients()
        {
            
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand
                    ("SELECT ID, Name, MedicalRecordNumber, Age, Gender, AdmittingDiagnosis, AttendingPhysician, Department FROM Patients", db);


                // Read advances through rows of returned data
                using (var reader = selectCommand.ExecuteReader())
                {
                    int id, age;
                    string name, medRecNum, gender, admittingDiagnosis, attendingPhysician, department;
                    Patient pat;
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        name = reader.GetString(1);
                        medRecNum = reader.GetString(2);
                        age = reader.GetInt32(3);
                        gender = reader.GetString(4);
                        admittingDiagnosis = reader.GetString(5);
                        attendingPhysician = reader.GetString(6);
                        department = reader.GetString(7);

                        // the constructor calls GetPatientContacts
                        pat = new Patient(name, medRecNum, age, gender, admittingDiagnosis, attendingPhysician, department, id);
                        pat.Contacts = GetPatientContacts(pat.DBID);
                        App.AppPatients.Add(pat);
                    }
                }
            }

        }

        public static List<Contact> GetPatientContacts(int DBID)
        {
            List<Contact> lc = new List<Contact>();

            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                var selectCommand = new SqliteCommand
                    ("SELECT ID, PatientID, Name, Relationship, ContactInfo FROM Contacts WHERE PatientID = @patDBID", db);

                selectCommand.Parameters.AddWithValue("@patDBID", DBID);
                // Read advances through rows of returned data
                using (var reader = selectCommand.ExecuteReader())
                {
                    int id, patID;
                    string name, relationship, contactInfo;
                    
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        patID = reader.GetInt32(1);
                        name = reader.GetString(2);
                        relationship = reader.GetString(3);
                        contactInfo = reader.GetString(4);


                        lc.Add(new Contact(name, relationship, contactInfo, patID, id));
                    }
                }
            }


            return lc;

        }

        

        
        // Adds Patient to database, updates paramter patient with database index
        public static void AddPatient(Patient pat)
        {
            
            using (var connection = new SqliteConnection($"Filename={dbpath}"))
            {
                connection.Open();

                var insertCommand = new SqliteCommand();
                insertCommand.Connection = connection;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = @"INSERT INTO Patients (
                                                Name, MedicalRecordNumber, Age, Gender, AdmittingDiagnosis, AttendingPhysician, Department) 
                                            VALUES (
                                                @name, @medicalRecNum, @age, @gender, @admittingDiagnosis, @attendingPhysician, @department);";
                insertCommand.Parameters.AddWithValue("@name", pat.Name);
                insertCommand.Parameters.AddWithValue("@medicalRecNum", pat.MedicalRecordNumber);
                insertCommand.Parameters.AddWithValue("@age", pat.Age);
                insertCommand.Parameters.AddWithValue("@gender", pat.Gender);
                insertCommand.Parameters.AddWithValue("@admittingDiagnosis", pat.AdmittingDiagnosis);
                insertCommand.Parameters.AddWithValue("@attendingPhysician", pat.AttendingPhysician);
                insertCommand.Parameters.AddWithValue("@department", pat.Department);

                insertCommand.ExecuteNonQuery();
            }

            // get patDBID of newly added patient
            pat.DBID = getPatientDBID(pat);

            if (pat.Contacts.Count > 0)
            {
                foreach (var contact in pat.Contacts)
                {
                    contact.PatientDBID = pat.DBID;
                    AddContact(contact, pat.DBID);
                }
            }

        }

        // Adds Contact to database, updates contact param with database index
        public static void AddContact(Contact contact, int patDBID)
        {
            using (var connection = new SqliteConnection("Data Source=patients.db"))
            {
                connection.Open();

                string insertContact = @"
                        INSERT INTO Contacts (PatientID, Name, Relationship, ContactInfo)
                        VALUES (@patientID, @name, @relationship, @contactInfo);";

                using (var command = new SqliteCommand(insertContact, connection))
                {
                    command.Parameters.AddWithValue("@patientID", patDBID);
                    command.Parameters.AddWithValue("@name", contact.Name);
                    command.Parameters.AddWithValue("@relationship", contact.Relationship);
                    command.Parameters.AddWithValue("@contactInfo", contact.ContactInfo);

                    command.ExecuteNonQuery();
                }
                contact.DBID = getContactDBID(contact);
            }
        }

        
        // update patient record, update existing contacts and add new contact records when applicable
        public static void UpdatePatient(Patient pat, Patient oldPat)
        {
            using (var connection = new SqliteConnection("Data Source=patients.db"))
            {
                connection.Open();

                string updatePatient = @"
                    UPDATE Patients
                    SET Name = @Name, MedicalRecordNumber = @medRecNum, Age = @Age, Gender = @Gender, AdmittingDiagnosis = @Diagnosis, AttendingPhysician = @attendingPhysician, Department = @department
                    WHERE Id = @Id;";

                using (var command = new SqliteCommand(updatePatient, connection))
                {
                    command.Parameters.AddWithValue("@Name", pat.Name);
                    command.Parameters.AddWithValue("@medRecNum", pat.MedicalRecordNumber);
                    command.Parameters.AddWithValue("@Age", pat.Age);
                    command.Parameters.AddWithValue("@Gender", pat.Gender);
                    command.Parameters.AddWithValue("@Diagnosis", pat.AdmittingDiagnosis);
                    command.Parameters.AddWithValue("@attendingPhysician", pat.AttendingPhysician);
                    command.Parameters.AddWithValue("@department", pat.Department);
                    command.Parameters.AddWithValue("@Id", pat.DBID);

                    command.ExecuteNonQuery();
                }

                // set to store contact databse IDs in updated contacts list
                HashSet<int> conDBIDSet = new HashSet<int>();

                List<Contact> newPatContactsList = pat.Contacts;
                List<Contact> oldPatContactsList = oldPat.Contacts;

                if (pat.Contacts.Count > 0)
                {
                    foreach (var contact in pat.Contacts)
                    {
                        System.Diagnostics.Debug.WriteLine($"newContact DBID {contact.DBID}");
                        conDBIDSet.Add(contact.DBID);
                        if (isContactInDatabase(contact.DBID))
                        {
                            UpdateContact(contact, contact.DBID);
                        }
                        else
                        {
                            AddContact(contact, contact.PatientDBID);
                        }
                    }
                }
                // Delete contacts that are no longer in the list
                if (oldPat.Contacts.Count > 0)
                {
                    foreach (var oldContact in oldPat.Contacts)
                    {
                        System.Diagnostics.Debug.WriteLine($"oldContact DBID {oldContact.DBID}");
                        if (!conDBIDSet.Contains(oldContact.DBID))
                        {
                            DeleteContact(oldContact.DBID);
                        }
                    }
                }

            }
        }

        public static void UpdateContact(Contact contact, int conDBID)
        {
            using (var connection = new SqliteConnection("Data Source=patients.db"))
            {
                connection.Open();

                string insertContact = @"
                        
                        UPDATE Contacts 
                        SET PatientID = @PatientID,
                            Relationship = @Relationship,
                            ContactInfo = @ContactInfo
                        WHERE ID = @DBID";

                using (var command = new SqliteCommand(insertContact, connection))
                {
                    command.Parameters.AddWithValue("@PatientID", contact.PatientDBID);
                    command.Parameters.AddWithValue("@Name", contact.Name);
                    command.Parameters.AddWithValue("@Relationship", contact.Relationship);
                    command.Parameters.AddWithValue("@ContactInfo", contact.ContactInfo);
                    command.Parameters.AddWithValue("@DBID", conDBID);

                    command.ExecuteNonQuery();
                }
            }
        }



        public static void DeletePatient(Patient pat)
        {

            using (var connection = new SqliteConnection($"Filename={dbpath}"))
            {
                connection.Open();

                // deleteCommand parameterized query to prevent SQL injection attacks
                string deletePatient = @"DELETE FROM Patients WHERE ID = @Id;";

                using (var command = new SqliteCommand(deletePatient, connection))
                {
                    command.Parameters.AddWithValue("@Id", pat.DBID);
                    command.ExecuteNonQuery();
                }

            }


        }

        public static void DeleteContact(int conDBID)
        {

            using (var connection = new SqliteConnection($"Filename={dbpath}"))
            {
                connection.Open();

                // deleteCommand parameterized query to prevent SQL injection attacks
                string deleteContact = @"DELETE FROM Contacts WHERE ID = @Id;";

                using (var command = new SqliteCommand(deleteContact, connection))
                {
                    command.Parameters.AddWithValue("@Id", conDBID);
                    command.ExecuteNonQuery();
                }

            }

        }

        public static void DeleteContacts(int patID)
        {

            using (var connection = new SqliteConnection($"Filename={dbpath}"))
            {
                connection.Open();

                // deleteCommand parameterized query to prevent SQL injection attacks
                string deleteContacts = @"DELETE FROM Contacts WHERE PatientID = @Id;";

                using (var command = new SqliteCommand(deleteContacts, connection))
                {
                    command.Parameters.AddWithValue("@Id", patID);
                    command.ExecuteNonQuery();
                }

            }

        }


        public static int getPatientDBID(Patient pat)
        {
            int id = -1;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var getIDCommand = new SqliteCommand();
                getIDCommand.Connection = db;

                getIDCommand.CommandText = @"SELECT ID FROM Patients WHERE MedicalRecordNumber = @medicalRecNum";
                getIDCommand.Parameters.AddWithValue("@medicalRecNum", pat.MedicalRecordNumber);
                using (var reader = getIDCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        System.Diagnostics.Debug.WriteLine($"id: {id}");
                    }
                }

            }

            return id;
        }

        public static int getContactDBID(Contact con)
        {
            int id = -1;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var getIDCommand = new SqliteCommand();
                getIDCommand.Connection = db;

                getIDCommand.CommandText = @"SELECT MAX(ID) FROM Contacts 
                                            WHERE PatientID = @pid 
                                            AND Name = @name 
                                            AND Relationship = @relationship 
                                            AND ContactInfo = @conInfo";
                getIDCommand.Parameters.AddWithValue("@pid", con.PatientDBID);
                getIDCommand.Parameters.AddWithValue("@name", con.Name);
                getIDCommand.Parameters.AddWithValue("@relationship", con.Relationship);
                getIDCommand.Parameters.AddWithValue("@conInfo", con.ContactInfo);

                using (var reader = getIDCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                        System.Diagnostics.Debug.WriteLine($"id: {id}");
                    }
                }

            }

            return id;
        }


        // checks the database to see if the passed medical record number would be unique if added in
        public static bool isUniqueMedRecNum(string medRecNum)
        {
            string result = "";
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var getMedRecNum = new SqliteCommand();
                getMedRecNum.Connection = db;

                getMedRecNum.CommandText = @"SELECT MedicalRecordNumber FROM Patients WHERE MedicalRecordNumber = @medicalRecNum";
                getMedRecNum.Parameters.AddWithValue("@medicalRecNum", medRecNum);
                using (var reader = getMedRecNum.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(0);
                    }
                }

            }

            if (result != "")
            {
                return false;
            }

            return true;
        }

        public static bool isContactInDatabase(int DBID)
        {
            if (DBID == -1)
            {
                return false;
            }

            int result = -1;
            using (var db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                var getMatchingContact = new SqliteCommand();
                getMatchingContact.Connection = db;

                getMatchingContact.CommandText = @"SELECT ID FROM Contacts WHERE ID = @DBID";
                getMatchingContact.Parameters.AddWithValue("@DBID", DBID);
                using (var reader = getMatchingContact.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }

            }

            if (result == -1)
            {
                return false;
            }

            return true;
        }

    }

    public static class PatientsOpe 
    {
        public static void AddPatient(Patient pat)
        {
            App.AppPatients.Add(pat);
            DataAccess.AddPatient(pat);
        }
        public static void DeletePatient(Patient pat)
        {
            App.AppPatients.Remove(pat);
            DataAccess.DeletePatient(pat);
        }
        public static void UpdatePatient(Patient pat, Patient oldPat)
        {
            int index = App.AppPatients.IndexOf(oldPat);
            if (index != -1)
            {
                App.AppPatients.RemoveAt(index);
                App.AppPatients.Add(pat);
            }
            DataAccess.UpdatePatient(pat, oldPat);
        }
    }



    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            DataAccess.InitializeDatabase();
            System.Diagnostics.Debug.WriteLine("InitializeDatabase()");
            DataAccess.GetAllPatients();

            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            



        }

       

        private Window? m_window;
        public static Frame MainAppFrame { get; set; }

        public static ObservableCollection<Patient> AppPatients { get; set; } = new ObservableCollection<Patient>();


    }
}
