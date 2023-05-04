namespace AwsTestingSolution.Storages
{
    public class CloudximageDataStorage
    {
        public static string AppInstancePublicDns => "http://ec2-52-55-15-5.compute-1.amazonaws.com";
        public static string AppInstancePublicIp => "52.55.15.5";
        public static string PathToImageToBeUploaded => "F:\\AWS\\AwsTestingSolution\\AwsTestingSolution\\Files\\aws-free-logo.png";

        public static string DataBaseHostName => "cloudximage-databasemysqlinstanced64026b2-xtd7oyd4swx0.cbsm4cifcp5v.us-east-1.rds.amazonaws.com";
        public static string DataBaseUserName => "mysql_admin";
        public static string DataBaseName => "cloudximages";
        public static string DataBasePassword => Environment.GetEnvironmentVariable("RdsDBPassword");
        public static uint MySqlPort => 3306;
    }
}
