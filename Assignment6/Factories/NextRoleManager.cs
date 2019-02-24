using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment6.Factories
{
    public class NextRoleManagerFactory
    {
        public NextRoleManager Get(string role)
        {
            NextRole nextRole;
            switch (role)
            {
                case "Analyst":
                    nextRole = new AnalystNextRole();
                    break;
                case "Architect":
                    nextRole = new ArchitectNextRole();
                    break;
                case "Programmer":
                    nextRole = new ProgrammerNextRole();
                    break;
                case "Tester":
                    nextRole = new TesterNextRole();
                    break;
                default:
                    nextRole = new AnalystNextRole();
                    break;
                    //case "Manager":
                    //    nextRole = new AnalystNextRole();
                    //    break;
            }

            return new NextRoleManager(nextRole);
        }
    }
    public class NextRoleManager
    {
        private NextRole _nextRole;
        public NextRoleManager(NextRole nextRole)
        {
            _nextRole = nextRole;
        }
        public int Get(int currentRoleId)
        {
            return _nextRole.Get(currentRoleId);
        }
    }
    public abstract class NextRole
    {
        public abstract int Get(int currentRoleId);
    }
    public class AnalystNextRole : NextRole
    {
        public override int Get(int currentRoleId)
        {
            return (int)Roles.Architect;
        }
    }
    public class ArchitectNextRole : NextRole
    {
        public override int Get(int currentRoleId)
        {
            return (int)Roles.Programmer;
        }
    }
    public class ProgrammerNextRole : NextRole
    {
        public override int Get(int currentRoleId)
        {
            return (int)Roles.Tester;
        }
    }
    public class TesterNextRole : NextRole
    {
        public override int Get(int currentRoleId)
        {
            return (int)Roles.Tester;
        }
    }
    public enum Roles
    {
        Manager=1,
        Architect,
        Analyst,
        Programmer,
        Tester

    }
}