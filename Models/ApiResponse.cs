using System;

namespace InventoryPrototype.Models {

  [WithTypeScript]
  public class ApiResponse<T> {
    //helpers/fetchApi assumes non null Data upon successful response to callers
    [NotNullable]
    public T Data { get; set; }
    public bool Success => ErrorMessages == null || ErrorMessages.Length == 0;
    public string[] ErrorMessages { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data) {
      Data = data;
    }
  }
}
