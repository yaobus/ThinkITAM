namespace ThinkITAM.ViewModels.Preset;
public class PeopleViewModel
{
    //索引
    public float Index
    {
        get; set;
    }

    public string UserId
    {
        get; set;
    }

    //索引
    public string Number
    {
        get; set;
    }

    //索引
    public string UserNumber
    {
        get; set;
    }

    //姓名
    public string Name
    {
        get; set;
    }

    //组织
    public string Organization
    {
        get; set;
    }

    //部门
    public string Department
    {
        get; set;
    }

    public string Group
    {
        get; set;
    }

    public string Unit
    {
        get; set;
    }

    //组织
    public string Phone
    {

        get; set;
    }

    //备注
    public string Note
    {
        get; set;
    }

}


public class PeopleImportViewModel
{
    public string UserId
    {
        get;
        set;
    }

    public int Number
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }


    public string Organization
    {
        get;
        set;
    }

    public string Department
    {
        get;
        set;
    }

    public string UserGroup
    {
        get;
        set;
    }
    public string UserUnit
    {
        get;
        set;
    }

    public string Phone
    {
        get;
        set;
    }

    public string Note
    {
        get;
        set;
    }
}