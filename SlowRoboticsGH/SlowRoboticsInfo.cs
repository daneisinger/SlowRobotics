using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace SlowRobotics
{
    public class SlowRoboticsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SlowRobotics";
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
                return new Guid("54f0c10f-a33f-40f0-b091-cfdd17e143f4");
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
