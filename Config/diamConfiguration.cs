using System.Configuration;

namespace CoreSystemII.Config
{
    public class DiamConfiguration : ConfigurationSection
    {


        public static DiamConfiguration GetConfig()
        {
            return ConfigurationManager.GetSection("DiamPlanogramConfiguration") as DiamConfiguration;
        }

        [ConfigurationProperty("sessionName", DefaultValue = "sessionId", IsRequired = true)]
        public string SessionName
        {
            get
            {
                return this["sessionName"] as string;
            }
        }

        [ConfigurationProperty("adminRole", DefaultValue = "Administrator", IsRequired = true)]
        public string AdminRole
        {
            get
            {
                return this["adminRole"] as string;
            }
        }

        [ConfigurationProperty("diamRoles", DefaultValue = "Diam Sales", IsRequired = true)]
        public string DiamRoles
        {
            get
            {
                return this["diamRoles"].ToString();
            }
        }
        [ConfigurationProperty("clientValidatorRoles", DefaultValue = "Regional Manager", IsRequired = true)]
        public string ClientValidatorRoles
        {
            get
            {
                return this["clientValidatorRoles"].ToString();
            }
        }
        [ConfigurationProperty("clientEditorRoles", DefaultValue = "Client Super User", IsRequired = true)]
        public string ClientEditorRoles
        {
            get
            {
                return this["clientEditorRoles"].ToString();
            }
        }


        [ConfigurationProperty("Administrator", DefaultValue = "Administrator", IsRequired = true)]
        public string Administrator
        {
            get
            {
                return this["Administrator"].ToString();
            }
        }
        [ConfigurationProperty("DiamSuperUser", DefaultValue = "DiamSuperUser", IsRequired = true)]
        public string DiamSuperUser
        {
            get
            {
                return this["DiamSuperUser"].ToString();
            }
        }

        [ConfigurationProperty("Validator", DefaultValue = "Diam Sales", IsRequired = true)]
        public string Validator
        {
            get
            {
                return this["Validator"].ToString();
            }
        }
        [ConfigurationProperty("Approver", DefaultValue = "Regional Manager", IsRequired = true)]
        public string Approver
        {
            get
            {
                return this["Approver"].ToString();
            }
        }
        [ConfigurationProperty("Editor", DefaultValue = "Client", IsRequired = true)]
        public string Editor
        {
            get
            {
                return this["Editor"].ToString();
            }
        }

        [ConfigurationProperty("Shopper", DefaultValue = "Shopper", IsRequired = true)]
        public string Shopper
        {
            get
            {
                return this["Shopper"].ToString();
            }
        }

        [ConfigurationProperty("AdminShopper", DefaultValue = "AdminShopper", IsRequired = true)]
        public string AdminShopper
        {
            get
            {
                return this["AdminShopper"].ToString();
            }
        }

        [ConfigurationProperty("v2", DefaultValue = "Plan_X", IsRequired = true)]
        public string Plan_X
        {
            get
            {
                return this["v2"].ToString();
            }
        }
    }

    public class DiamEmailConfiguration : ConfigurationSection
    {


        public static DiamEmailConfiguration GetConfig()
        {
            return ConfigurationManager.GetSection("DiamEmailConfiguration") as DiamEmailConfiguration;
        }

        [ConfigurationProperty("FromAddress", DefaultValue = "noreply@diamdiagramm.com", IsRequired = true)]
        public string FromAddress
        {
            get
            {
                return this["FromAddress"] as string;
            }
        }

        [ConfigurationProperty("ToAddress", DefaultValue = "all.diamuk.webgroup@diaminter.com", IsRequired = true)]
        public string ToAddress
        {
            get
            {
                return this["ToAddress"].ToString();
            }
        }

        [ConfigurationProperty("RecipientName", DefaultValue = "uk web team", IsRequired = true)]
        public string RecipientName
        {
            get
            {
                return this["RecipientName"].ToString();
            }
        }
        [ConfigurationProperty("BCCList", DefaultValue = "", IsRequired = false)]
        public string BCCList
        {
            get
            {
                return this["BCCList"].ToString();
            }
        }
        [ConfigurationProperty("EmailEnabled", DefaultValue = "false", IsRequired = true)]
        public bool EmailEnabled
        {
            get
            {
                return (bool) (this["EmailEnabled"]);
            }
        }
    }

}