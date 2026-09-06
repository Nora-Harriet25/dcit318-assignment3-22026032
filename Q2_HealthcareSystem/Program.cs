using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // ===== a. Generic repository for managing any entity type =====
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }

        // Returns the first item matching the predicate, or null if none found
        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        // Removes the first item matching the predicate; returns true if something was removed
        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item is null)
                return false;

            return items.Remove(item);
        }
    }

    // ===== b. Patient class =====
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // ===== c. Prescription class =====
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // ===== g. HealthSystemApp - integrates everything =====
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            // Add sample patients
            _patientRepo.Add(new Patient(1, "Ama Owusu", 34, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Boateng", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Efua Mensah", 28, "Female"));

            // Add sample prescriptions (valid PatientIds matching patients above)
            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Metformin", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(5, 2, "Vitamin C", DateTime.Now.AddDays(-1)));
        }

        // ===== d, e. Build the dictionary grouping prescriptions by PatientId =====
        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }

                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        // ===== f. Retrieve prescriptions for a given patient from the map =====
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var prescriptions)
                ? prescriptions
                : new List<Prescription>();
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var prescriptions = GetPrescriptionsByPatientId(id);

            Console.WriteLine($"\n=== Prescriptions for Patient ID {id} ===");
            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.");
                return;
            }

            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"Prescription ID: {prescription.Id}, Medication: {prescription.MedicationName}, Date Issued: {prescription.DateIssued:d}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Healthcare System ===\n");

            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();

            app.PrintAllPatients();

            // Select one patient ID and display their prescriptions
            app.PrintPrescriptionsForPatient(2);
        }
    }
}
