using COCOApp.Models.Validations;
using Microsoft.AspNetCore.Mvc;
namespace COCOApp.Models
{

    [ModelMetadataType(typeof(UserMetadata))]
    public partial class User
    {
        // Leave this class empty. The validation rules will be applied from UserMetadata.
    }
}
