using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jsonzai.Test.Model;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.IO;

namespace Jsonzai.Test
{
    [TestClass]
    public class JsonParserTest
    {

        [TestMethod]
        public void TestParsingStudent()
        {
            string src = "{Name: \"Ze Manel\", Nr: 6512, Group: 11, github_id: \"omaior\"}";
            Student std = JsonParsemit.Parse<Student>(src);            
            Assert.AreEqual("Ze Manel", std.Name);
            Assert.AreEqual(6512, std.Nr);
            Assert.AreEqual(11, std.Group);
            Assert.AreEqual("omaior", std.GithubId);
        }
        [TestMethod]
        public void TestSiblings()
        {
            string src = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\", Sibling: { Name: \"Kata Badala\"}}}";
            Person p = JsonParsemit.Parse<Person>(src);
            Assert.AreEqual("Ze Manel", p.Name);
            Assert.AreEqual("Maria Papoila", p.Sibling.Name);
            Assert.AreEqual("Kata Badala", p.Sibling.Sibling.Name);
        }

        [TestMethod]
        public void TestParsingPersonWithBirth()
        {
            string src = "{Name: \"Ze Manel\", Birth: {Year: 1999, Month: 12, Day: 31}}";
            Person p = JsonParsemit.Parse<Person>(src);
            Assert.AreEqual("Ze Manel", p.Name);
            Assert.AreEqual(1999, p.Birth.Year);
            Assert.AreEqual(12, p.Birth.Month);
            Assert.AreEqual(31, p.Birth.Day);
        }

        [TestMethod]
        public void TestParsingPersonArray()
        {
            string src = "[{Name: \"Ze Manel\"}, {Name: \"Candida Raimunda\"}, {Name: \"Kata Mandala\"}]";
            Person[] ps = JsonParsemit.Parse<Person[]>(src);
            Assert.AreEqual(3, ps.Length);
            Assert.AreEqual("Ze Manel", ps[0].Name);
            Assert.AreEqual("Candida Raimunda", ps[1].Name);
            Assert.AreEqual("Kata Mandala", ps[2].Name);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestBadJsonObjectWithUnclosedBrackets()
        {
            string src = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\", Sibling: { Name: \"Kata Badala\"}";
            Person p = JsonParsemit.Parse<Person>(src);
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBadJsonObjectWithWrongCloseToken()
        {
            string src = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\"]]";
            Person p = JsonParsemit.Parse<Person>(src);
        }
        [TestMethod]
        public void TestParsingStudentArray()
        {
            Student s1 = new Student();
            s1.Name = "Maria Castro";
            s1.Nr = 44531;
            s1.Group = 12;
            s1.GithubId = "mcastro";
            Student s2 = new Student();
            s2.Name = "Manel Castro";
            s2.Nr = 44532;
            s2.Group = 12;
            s2.GithubId = "mncastro";
            Student s3 = new Student();
            s3.Name = "Manel Pedro";
            s3.Nr = 44533;
            s3.Group = 12;
            s3.GithubId = "mpedro";

            Student[] Classroom = { s1, s2, s3 };
            string json = JsonConvert.SerializeObject(Classroom);
            json = json.Replace("GithubId", "github_id");
            Student[] classroom = JsonParsemit.Parse<Student[]>(json);
            for (int i = 0; i < Classroom.Length; i++)
            {
                Assert.AreEqual(Classroom[i].Name, classroom[i].Name);
                Assert.AreEqual(Classroom[i].Nr, classroom[i].Nr);
                Assert.AreEqual(Classroom[i].GithubId, classroom[i].GithubId);
                Assert.AreEqual(Classroom[i].Group, classroom[i].Group);
            }
        }
        [TestMethod]
        public void TestParsingClassroom()
        {
            Classroom cls = new Classroom();
            cls.Class = "LI41N";
            Student s1 = new Student();
            s1.Name = "Maria Castro";
            s1.Nr = 44531;
            s1.Group = 12;
            s1.GithubId = "mcastro";
            Student s2 = new Student();
            s2.Name = "Manel Castro";
            s2.Nr = 44532;
            s2.Group = 12;
            s2.GithubId = "mncastro";
            Student s3 = new Student();
            s3.Name = "Manel Pedro";
            s3.Nr = 44533;
            s3.Group = 12;
            s3.GithubId = "mpedro";
            cls.Student = new Student[] { s1, s2, s3 };
            string json = JsonConvert.SerializeObject(cls);
            json = json.Replace("GithubId", "github_id");
            Classroom classroom = JsonParser.Parse<Classroom>(json);
            Assert.AreEqual(cls.Class, classroom.Class);
            for (int i = 0; i < cls.Student.Length; ++i)
            {
                Assert.AreEqual(cls.Student[i].Name, classroom.Student[i].Name);
                Assert.AreEqual(cls.Student[i].Nr, classroom.Student[i].Nr);
                Assert.AreEqual(cls.Student[i].GithubId, classroom.Student[i].GithubId);
                Assert.AreEqual(cls.Student[i].Group, classroom.Student[i].Group);
            }
        }
        [TestMethod]
        public void TestParsingAccount()
        {
            Account acc = new Account();
            acc.Balance = 234.32;
            acc.Transactions = new Double[] { -100.00, 32.00, -5.00 };
            string json = JsonConvert.SerializeObject(acc);
            Account account = JsonParsemit.Parse<Account>(json);
            Assert.AreEqual(acc.Balance, account.Balance);
            for (int i = 0; i < acc.Transactions.Length; ++i)
                Assert.AreEqual(acc.Transactions[i], account.Transactions[i]);
        }
        [TestMethod]
        public void TestJsonProperty()
        {
            string src = "{Name: \"Ze Manel\", Nr: 6512, Group: 11, github_id: \"omaior\"}";
            Student std = JsonParsemit.Parse<Student>(src);
            Assert.AreEqual("Ze Manel", std.Name);
            Assert.AreEqual(6512, std.Nr);
            Assert.AreEqual(11, std.Group);
            Assert.AreEqual("omaior", std.GithubId);
        }
        [TestMethod]
        public void TestJsonUri()
        {
            Website website = new Website();
            website.Uri = new Uri("https://www.google.com/");
            string json = JsonConvert.SerializeObject(website);
            json = json.Replace("GithubId", "github_id");
            Website web = JsonParsemit.Parse<Website>(json);
            Assert.AreEqual(website.Uri, web.Uri);

        }
        [TestMethod]
        public void TestJsonGuid()
        {
            Classroom cls = new Classroom();
            cls.Id = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            string json = JsonConvert.SerializeObject(cls);
            json = json.Replace("GithubId", "github_id");
            JsonParser.AddConfiguration<Classroom, Guid>("Id", JsonToGuid.Parse2);
            Classroom classroom = JsonParser.Parse<Classroom>(json);            
            Assert.AreEqual(cls.Id, classroom.Id);
        }
        [TestMethod]
        public void TestJsonDateTime()
        {
            Project prj = new Project();
            prj.Student = new Student();
            prj.Student.Name = "Maria Castro";
            prj.Student.Nr = 44531;
            prj.Student.Group = 12;
            prj.Student.GithubId = "mcastro";
            prj.DueDate = new DateTime(2019, 11, 14, 23, 59, 00);
            string json = JsonConvert.SerializeObject(prj);
            json = json.Replace("Student", "student_isel").Replace("GithubId", "github_id");
            Project project = JsonParsemit.Parse<Project>(json);
            Assert.AreEqual(prj.Student.Name, project.Student.Name);
            Assert.AreEqual(prj.Student.Nr, project.Student.Nr);
            Assert.AreEqual(prj.Student.Group, project.Student.Group);
            Assert.AreEqual(prj.Student.GithubId, project.Student.GithubId);
            Assert.AreEqual(prj.DueDate, project.DueDate);
        }

        [TestMethod]
        public void TestNumber()
        {
            Account acc = new Account();
            acc.Balance = 1063.64;
            acc.Transactions = new Double[] { -10.0, -32.45, +635 };
            acc.Iban =
                new Number("PT50", "1234 4321 12345678901 72", new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
            string json = JsonConvert.SerializeObject(acc);
            Account account = JsonParsemit.Parse<Account>(json);
            Assert.AreEqual(acc.Balance, account.Balance);
            for (int i = 0; i < acc.Transactions.Length; i++)
                Assert.AreEqual(acc.Transactions[i], account.Transactions[i]);
            Assert.AreEqual(acc.Iban.Prefix, account.Iban.Prefix);
            Assert.AreEqual(acc.Iban.Digits, account.Iban.Digits);
            Assert.AreEqual(acc.Iban.Id, account.Iban.Id);
        }
        [TestMethod]
        public void TestJsonStructArrayAgenda()
        {
            Agenda agenda = new Agenda();
            agenda.appointmentsSize = 3;
            agenda.appointments = new string[agenda.appointmentsSize];
            agenda.appointments[0] = "Dentist";
            agenda.appointments[1] = "Hairdresser";
            agenda.appointments[2] = "English Class";
            string json = JsonConvert.SerializeObject(agenda);
            Agenda ag = JsonParsemit.Parse<Agenda>(json);
            for (int i = 0; i < agenda.appointmentsSize; i++)
                Assert.AreEqual(agenda.appointments[i], ag.appointments[i]);
            Assert.AreEqual(agenda.appointmentsSize, ag.appointmentsSize);
        }

        [TestMethod]
        public void TestJsonStructArrayTypeValueSpeedTest()
        {
            SpeedTest speedtest = new SpeedTest();
            Student st1 = new Student();
            st1.Name = "Maria Castro";
            st1.Nr = 44567;
            st1.Group = 12;
            speedtest.Student = st1;
            Double[] speedvalue1 = { 12.4, 10.5, 9.7 };
            speedtest.Speedval = speedvalue1;
            string json = JsonConvert.SerializeObject(speedtest).Replace("GithubId", "github_id");
            SpeedTest st = JsonParsemit.Parse<SpeedTest>(json);
            Assert.AreEqual(speedtest.Student.Name, st.Student.Name);
            Assert.AreEqual(speedtest.Student.Nr, st.Student.Nr);
            Assert.AreEqual(speedtest.Student.Group, st.Student.Group);
            for (int i = 0; i < speedtest.Speedval.Length; i++)
                Assert.AreEqual(speedtest.Speedval[i], st.Speedval[i]);

        }
        [TestMethod]
        public void LazySequenceTest()
        {

            StreamWriter writer = new StreamWriter(new FileStream("test.txt", FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
            StreamReader reader = new StreamReader(new FileStream("test.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            String content = reader.ReadToEnd();
            String oldContent = content;
            IEnumerator<Person> enumerator = JsonParser.SequenceFrom<Person>("test.txt").GetEnumerator();                                                                   
            enumerator.MoveNext();
            Assert.AreEqual("Ze Manel", enumerator.Current.Name);
            content = content.Replace("Candida Raimunda", "Albertina Asdrubal");
            writer.Write(content);
            writer.Close();
            enumerator.MoveNext();
            Assert.AreEqual("Albertina Asdrubal", enumerator.Current.Name);
            writer = new StreamWriter(new FileStream("test.txt", FileMode.Open, FileAccess.Write, FileShare.ReadWrite));
            writer.Write(oldContent);
            writer.Close();
            reader.Close();
        }

    }
}
