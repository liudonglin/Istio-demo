using System;
using System.Collections.Generic;

public class AppEntity
{
    public int AppID { get; set; }

    public string AppImage { get; set; }

    public string AppName { get; set; }

    public string AppDescription { get; set; }

    public decimal AppPrice { get; set; }

    public DateTime PublishDate { get; set; }

    public List<OrderInfo> Orders { get; set; }
}