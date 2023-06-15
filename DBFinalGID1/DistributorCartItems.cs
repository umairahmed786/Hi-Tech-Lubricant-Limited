using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace DBFinalGID1
{
    public class DistributorCartItems
    {
        public string productName;
        public string productQuantity;
        public string productPerLitrePrice;
        public string productVolume;
        public string productTotalPrice;
        public Image productImage;
        public float totalQuantityPresentInStock;


        public DistributorCartItems(Image prodImage ,string productName ,string productVolume ,string productQuantity , string productPerLitrePrice , string productTotalPrice , float totalQuantityPresentInStock)
        {
            this.totalQuantityPresentInStock = totalQuantityPresentInStock;
            this.productName = productName;
            this.productQuantity = productQuantity;
            this.productPerLitrePrice = productPerLitrePrice;
            this.productVolume = productVolume;
            this.productTotalPrice = productTotalPrice;
            this.productImage = prodImage;
        }
    }
}
