using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Jsonzai;
using Jsonzai.Test.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonzaiBenchmark
{
    public class JsonzaiBenchmarkConfig : ManualConfig
    {
        public JsonzaiBenchmarkConfig()
        {
            Add(Job.MediumRun
                   .WithLaunchCount(1)
                   .With(InProcessEmitToolchain.Instance)
                   .WithId("InProcess"));
        }
    }

    [RankColumn]
    [Config(typeof(JsonzaiBenchmarkConfig))]

    public class JsonzaiBenchmark
    {
        string benchStudent = "{Name: \"Ze Manel\", Nr: 6512, Group: 11, github_id: \"omaior\"}";
        string benchSiblings = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\", Sibling: { Name: \"Kata Badala\"}}}";
        string benchPersonWithBirth = "{Name: \"Ze Manel\", Birth: {Year: 1999, Month: 12, Day: 31}}";
        string benchPersonArray = "[{Name: \"Ze Manel\"}, {Name: \"Candida Raimunda\"}, {Name: \"Kata Mandala\"}]";



        public string BenchStudentArray()
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
            return json;
        }

        public string BenchClassroom()
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
            return json;
        }

        public string BenchAccount()
        {
            Account acc = new Account();
            acc.Balance = 234.32;
            acc.Transactions = new Double[] { -100.00, 32.00, -5.00 };
            string json = JsonConvert.SerializeObject(acc);
            return json;
        }

        public string BenchJsonUri()
        {
            Website website = new Website();
            website.Uri = new Uri("https://www.google.com/");
            string json = JsonConvert.SerializeObject(website);
            json = json.Replace("GithubId", "github_id");
            return json;
        }

        public string BenchJsonGuid()
        {
            Classroom cls = new Classroom();
            cls.Id = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            string json = JsonConvert.SerializeObject(cls);
            json = json.Replace("GithubId", "github_id");
            return json;
        }

        public string BenchJsonDatetime()
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
            return json;
        }

        public string BenchTestNumber()
        {
            Account acc = new Account();
            acc.Balance = 1063.64;
            acc.Transactions = new Double[] { -10.0, -32.45, +635 };
            acc.Iban = new Number("PT50", "1234 4321 12345678901 72", new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"));
            string json = JsonConvert.SerializeObject(acc);
            return json;
        }

        public string BenchStructArrayAgenda()
        {
            Agenda agenda = new Agenda();
            agenda.appointmentsSize = 3;
            agenda.appointments = new string[agenda.appointmentsSize];
            agenda.appointments[0] = "Dentist";
            agenda.appointments[1] = "Hairdresser";
            agenda.appointments[2] = "English Class";
            string json = JsonConvert.SerializeObject(agenda);
            return json;
        }

        public string BenchStructArrayTypeValueSpeedTest()
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
            return json;
        }
        [Benchmark]
        public void BenchStudentReflect()
        {
            JsonParser.Parse(benchStudent, typeof(Student));
        }

        [Benchmark]
        public void BenchStudentEmit()
        {
            JsonParsemit.Parse(benchStudent, typeof(Student));
        }

        [Benchmark]
        public void BenchStudentArrayReflect()
        {
            JsonParser.Parse(BenchStudentArray(), typeof(Student));
        }

        [Benchmark]
        public void BenchStudentArrayEmit()
        {
            JsonParsemit.Parse(BenchStudentArray(), typeof(Student));
        }
        [Benchmark]
        public void BenchClassroomReflect()
        {
            JsonParser.Parse(BenchClassroom(), typeof(Classroom));
        }

        [Benchmark]
        public void BenchClassroomEmit()
        {
            JsonParsemit.Parse(BenchClassroom(), typeof(Classroom));
        }
        [Benchmark]
        public void BenchAccountReflect()
        {
            JsonParser.Parse(BenchAccount(), typeof(Account));
        }

        [Benchmark]
        public void BenchAccountEmit()
        {
            JsonParsemit.Parse(BenchAccount(), typeof(Account));
        }
        [Benchmark]
        public void BenchPropertyReflect()
        {
            JsonParser.Parse(benchStudent, typeof(Student));
        }

        [Benchmark]
        public void BenchPropertyEmit()
        {
            JsonParsemit.Parse(benchStudent, typeof(Student));
        }

        [Benchmark]
        public void BenchSiblingsReflect()
        {
            JsonParser.Parse(benchSiblings, typeof(Person));
        }

        [Benchmark]
        public void BenchSiblingsEmit()
        {
            JsonParsemit.Parse(benchSiblings, typeof(Person));
        }

        [Benchmark]
        public void BenchPersonWithBirthReflect()
        {
            JsonParser.Parse(benchPersonWithBirth, typeof(Person));
        }

        [Benchmark]
        public void BenchPersonWithBirthEmit()
        {
            JsonParsemit.Parse(benchPersonWithBirth, typeof(Person));
        }

        [Benchmark]
        public void BenchPersonArrayReflect()
        {
            JsonParser.Parse(benchPersonArray, typeof(Person));
        }

        [Benchmark]
        public void BenchPersonArrayEmit()
        {
            JsonParsemit.Parse(benchPersonArray, typeof(Person));
        }

        [Benchmark]
        public void BenchJsonUriReflect()
        {
            JsonParser.Parse(BenchJsonUri(), typeof(Website));
        }

        [Benchmark]
        public void BenchJsonUriEmit()
        {
            JsonParsemit.Parse(BenchJsonUri(), typeof(Website));
        }

        [Benchmark]
        public void BenchJsonGuidReflect()
        {
            JsonParser.Parse(BenchJsonGuid(), typeof(Classroom));
        }

        [Benchmark]
        public void BenchJsonGuidEmit()
        {
            JsonParsemit.Parse(BenchJsonGuid(), typeof(Classroom));
        }

        [Benchmark]
        public void BenchJsonDatetimeReflect()
        {
            JsonParser.Parse(BenchJsonDatetime(), typeof(Project));
        }

        [Benchmark]
        public void BenchJsonDatetimeEmit()
        {
            JsonParsemit.Parse(BenchJsonDatetime(), typeof(Project));
        }

        [Benchmark]
        public void BenchTestNumberReflect()
        {
            JsonParser.Parse(BenchTestNumber(), typeof(Account));
        }

        [Benchmark]
        public void BenchTestNumberEmit()
        {
            JsonParsemit.Parse(BenchTestNumber(), typeof(Account));
        }
        [Benchmark]
        public void BenchStructArrayAgendaReflect()
        {
            JsonParser.Parse(BenchStructArrayAgenda(), typeof(Agenda));
        }

        [Benchmark]
        public void BenchStructArrayAgendaEmit()
        {
            JsonParsemit.Parse(BenchStructArrayAgenda(), typeof(Agenda));
        }
        [Benchmark]
        public void BenchStructArrayTypeValueSpeedTestReflect()
        {
            JsonParser.Parse(BenchStructArrayTypeValueSpeedTest(), typeof(SpeedTest));
        }

        [Benchmark]
        public void BenchStructArrayTypeValueSpeedTestEmit()
        {
            JsonParsemit.Parse(BenchStructArrayTypeValueSpeedTest(), typeof(SpeedTest));
        }
    }
}
