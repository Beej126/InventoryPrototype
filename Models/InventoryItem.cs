using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryPrototype.Models {
  [WithTypeScript]
  public class InventoryItem {
    [Key]
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
  }

}
