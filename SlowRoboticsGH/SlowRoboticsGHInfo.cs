using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace SlowRoboticsGH
{
    public class SlowRoboticsGHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SlowRoboticsGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("da013e7e-57c3-4977-956d-8aecaee99f88");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
