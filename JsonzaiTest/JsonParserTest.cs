﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jsonzai.Test.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Jsonzai.Test
{
    [TestClass]
    public class JsonParserTest
    {
        [TestMethod]
        public void TestParsingStudent()
        {
            string src = "{Name: \"Ze Manel\", Nr: 6512, Group: 11, github_id: \"omaior\"}";
            Student std = (Student) JsonParser.Parse(src, typeof(Student));
            Assert.AreEqual("Ze Manel", std.Name);
            Assert.AreEqual(6512, std.Nr);
            Assert.AreEqual(11, std.Group);
            Assert.AreEqual("omaior", std.GithubId);
        }
        [TestMethod]
        public void TestSiblings()
        {
            string src = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\", Sibling: { Name: \"Kata Badala\"}}}";
            Person p = (Person)JsonParser.Parse(src, typeof(Person));
            Assert.AreEqual("Ze Manel", p.Name);
            Assert.AreEqual("Maria Papoila", p.Sibling.Name);
            Assert.AreEqual("Kata Badala", p.Sibling.Sibling.Name);
        }

        [TestMethod]
        public void TestParsingPersonWithBirth()
        {
            string src = "{Name: \"Ze Manel\", Birth: {Year: 1999, Month: 12, Day: 31}}";
            Person p = (Person)JsonParser.Parse(src, typeof(Person));
            Assert.AreEqual("Ze Manel", p.Name);
            Assert.AreEqual(1999, p.Birth.Year);
            Assert.AreEqual(12, p.Birth.Month);
            Assert.AreEqual(31, p.Birth.Day);
        }

        [TestMethod]
        public void TestParsingPersonArray()
        {
            string src = "[{Name: \"Ze Manel\"}, {Name: \"Candida Raimunda\"}, {Name: \"Kata Mandala\"}]";
            Person [] ps = (Person[]) JsonParser.Parse(src, typeof(Person));
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
            Person p = (Person)JsonParser.Parse(src, typeof(Person));
        }
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBadJsonObjectWithWrongCloseToken()
        {
            string src = "{Name: \"Ze Manel\", Sibling: { Name: \"Maria Papoila\"]]";
            Person p = (Person)JsonParser.Parse(src, typeof(Person));
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
            Student[] classroom = (Student[])JsonParser.Parse(json, typeof(Student));
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
            Classroom classroom = (Classroom)JsonParser.Parse(json, typeof(Classroom));
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
            Account account = (Account)JsonParser.Parse(json, typeof(Account));
            Assert.AreEqual(acc.Balance, account.Balance);
            for(int i = 0; i < acc.Transactions.Length; ++i)
            {
                Assert.AreEqual(acc.Transactions[i], account.Transactions[i]);
            }            
        }        
        [TestMethod]
        public void TestJsonProperty()
        {
            string src = "{Name: \"Ze Manel\", Nr: 6512, Group: 11, github_id: \"omaior\"}";
            Student std = (Student)JsonParser.Parse(src, typeof(Student));
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
            Website web = (Website)JsonParser.Parse(json, typeof(Website));
            Assert.AreEqual(website.Uri, web.Uri);

        }
        [TestMethod]
        public void TestJsonGuid()
        {
            Classroom cls = new Classroom();
            cls.Id = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4");
            string json = JsonConvert.SerializeObject(cls);
            json = json.Replace("GithubId", "github_id");
            Classroom classroom = (Classroom)JsonParser.Parse(json, typeof(Classroom));
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
            json = json.Replace("Student", "student_isel").Replace("GithubId","github_id");
            Project project = (Project)JsonParser.Parse(json, typeof(Project));
            Assert.AreEqual(prj.Student.Name, project.Student.Name);
            Assert.AreEqual(prj.Student.Nr, project.Student.Nr);
            Assert.AreEqual(prj.Student.Group, project.Student.Group);
            Assert.AreEqual(prj.Student.GithubId, project.Student.GithubId);
            Assert.AreEqual(prj.DueDate, project.DueDate);

           
        }
    }
}