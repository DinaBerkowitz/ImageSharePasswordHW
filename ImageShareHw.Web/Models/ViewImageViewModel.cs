using ImageShareHW.Data;
using Microsoft.Extensions.Primitives;

namespace ImageShareHw.Web.Models
{
    public class ViewImageViewModel
    {


        public bool Unlocked { get; set; }
        public Images Image { get; set; }
        public string InvalidMessage { get; set; }
      

    }
}
