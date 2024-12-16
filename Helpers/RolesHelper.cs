using CoreSystemII.Config;

namespace CoreSystem2024.Helpers
{

    public enum Role : int
    {
        Administrator = 1,
        Validator = 2,
        Editor = 3,
        Approver = 4

    }
    public static class RolesHelper
    {

        //public static bool IsAdminUser()
        //{
        //    string str_adminRole = DiamConfiguration.GetConfig().AdminRole;
        //    int adminRole = int.Parse(str_adminRole);
        //    string[] str_roles = Roles.GetRolesForUser();
        //    int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));

        //    // Execute the following logic for Items and Alternating Items.
        //    if (roles.Contains(adminRole))
        //    {
        //        return true;
        //    }
        //    else
        //    { return false; }
        //}


        public static bool IsAdminUser(string Roles)
        {
            string adminRole = DiamConfiguration.GetConfig().AdminRole;
            string[] str_adminRoles = adminRole.Split(new char[] { ',' });
            int[] adminRoles = Array.ConvertAll(str_adminRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (int arole in adminRoles)
            {
                if (roles.Contains(arole))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDiamUser(string Roles)
        {
            string[] str_diamRoles = DiamConfiguration.GetConfig().DiamRoles.Split(new char[] { ',' });
            int[] diamRoles = Array.ConvertAll(str_diamRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));

            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (diamRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsClientEditor(string Roles)
        {
            string[] str_clientEditorRoles = DiamConfiguration.GetConfig().ClientEditorRoles.Split(new char[] { ',' });
            int[] clientEditorRoles = Array.ConvertAll(str_clientEditorRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));

            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (clientEditorRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsClientValidator(string Roles)
        {
            string[] str_clientValidatorRoles = DiamConfiguration.GetConfig().ClientValidatorRoles.Split(new char[] { ',' });
            int[] clientValidatorRoles = Array.ConvertAll(str_clientValidatorRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (clientValidatorRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsEditor(string Roles)
        {
            string[] str_editorRoles = DiamConfiguration.GetConfig().Editor.Split(new char[] { ',' });
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] editorRoles = Array.ConvertAll(str_editorRoles, s => int.Parse(s));
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (editorRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsApprover(string Roles)
        {
            string[] str_approverRoles = DiamConfiguration.GetConfig().Approver.Split(new char[] { ',' });
            int[] approverRoles = Array.ConvertAll(str_approverRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));

            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (approverRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsValidator(string Roles)
        {
            string[] str_validatorRoles = DiamConfiguration.GetConfig().Validator.Split(new char[] { ',' });
            int[] validatorRoles = Array.ConvertAll(str_validatorRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (int role in roles)
            {
                if (validatorRoles.Contains(role))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsAdministrator(string Roles)
        {
            string str_adminRole = DiamConfiguration.GetConfig().Administrator;
            //string[] str_adminRoles = adminRole.Split(new char[] { ',' });
            int adminRole = int.Parse(str_adminRole);
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            if (roles.Contains(adminRole))
            {
                return true;
            }
            return false;

        }
        public static bool IsSuperUser(string Roles)
        {
            string superUserRole = DiamConfiguration.GetConfig().DiamSuperUser;
            string[] str_userRoles = superUserRole.Split(new char[] { ',' });
            int[] userRoles = Array.ConvertAll(str_userRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (var arole in userRoles)
            {
                if (roles.Contains(arole))
                {
                    return true;
                }
            }
            return false;

        }
        public static bool IsShopper(string Roles)
        {
            string shopperRole = DiamConfiguration.GetConfig().Shopper;
            string[] str_shopperRoles = shopperRole.Split(new char[] { ',' });
            int[] shopperRoles = Array.ConvertAll(str_shopperRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (var arole in shopperRoles)
            {
                if (roles.Contains(arole))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsAdminShopper(string Roles)
        {
            string adminShopperRole = DiamConfiguration.GetConfig().AdminShopper;
            string[] str_adminShopperRoles = adminShopperRole.Split(new char[] { ',' });
            int[] adminShopperRoles = Array.ConvertAll(str_adminShopperRoles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (var arole in adminShopperRoles)
            {
                if (roles.Contains(arole))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPlanxUser(string Roles)
        {
            string plan_X = DiamConfiguration.GetConfig().Plan_X;
            string[] str_planxroles = plan_X.Split(new char[] { ',' });
            int[] planxroles = Array.ConvertAll(str_planxroles, s => int.Parse(s));
            string[] str_roles = Roles.Split(new char[] { ',' });
            int[] roles = Array.ConvertAll(str_roles, s => int.Parse(s));
            // Execute the following logic for Items and Alternating Items.
            foreach (var pxrole in planxroles)
            {
                if (roles.Contains(pxrole))
                {
                    return true;
                }
            }
            return false;

        }

        public static Role GetUserRole(string Roles)
        {
            if (IsAdministrator(Roles))
            {
                return Role.Administrator;
            }
            if (IsApprover(Roles))
            {
                return Role.Approver;
            }
            if (IsValidator(Roles))
            {
                return Role.Validator;
            }
            if (IsEditor(Roles))
            {
                return Role.Editor;
            }
            return Role.Editor;
        }
    }

}