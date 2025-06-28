namespace ThinkITAM.ViewModels.DatabaseEntity.Device;
public class DeviceViewModel
{
    //$"INSERT INTO \"Devices\" (\"AssetId\", \"AssetNumber\", \"AssetType\", \"DeviceType\", \"Model\",\"Description\", \"User\", \"UserPhone\", \"EnableDate\", \"UseDepartment\", \"Address\", \"TagA\", \"TagB\", \"TagC\", \"TagD\", \"TagE\", \"TagF\") VALUES ('{assetId}', '{assetNumber}', '{assetType}', '{deviceType}', '{model}','{description}', '{user}', '{userPhone}', '{enableDate}', '{userDepartment}', '{address}', '{tagA}', '{tagB}', '{tagC}', '{tagD}', '{tagE}', '{tagF}')";

    public string AssetId
    {
        get; set;

    }
    public string AssetNumber
    {
        get; set;
    }

    public string AssetType
    {
        get;
        set;
    }

    public string DeviceType
    {
        get;
        set;
    }

    public string Model
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    public string User
    {
        get; set;
    }

    public string UserPhone
    {
        get; set;
    }

    public string EnableDate
    {
        get;
        set;
    }

    public string DeviceRoom
    {
        get;
        set;
    }

    public string DeviceCabinet
    {
        get; set;

    }

    public string TagA
    {
        get; set;
    }
    public string TagB
    {
        get; set;
    }
    public string TagC
    {
        get; set;
    }
    public string TagD
    {
        get; set;
    }
    public string TagE
    {
        get; set;
    }
    public string TagF
    {
        get; set;
    }
}
