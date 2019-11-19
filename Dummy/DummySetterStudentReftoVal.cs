using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jsonzai;

namespace Jsonzai.Test.Model
{
    public interface ISetter
    {
        Type Klass { get; }
        object SetValue(object target, object value);
    }

    class DummySetterStudentTV : ISetter
    {
        public Type Klass { get => typeof(int); }

        public object SetValue(object target, object value)
        {
            ((Student)target).Nr = (int)value;
			return target;
        }
    }
    public class Student : Person
    {
        public Student() : base() { }
        public Student(int nr, string name, int group, string githubId) : base(name)
        {
            this.Nr = nr;
            this.Group = group;
            this.GithubId = githubId;
        }

        public int Nr { get; set; }

        public int Group { get; set; }
       
        public string GithubId { get; set; }
    }
    public class Person
    {
        public string Name { get; set; }

        public Person()
        {
        }

        public Person(string name)
        {
            this.Name = name;
        }
    }


}
