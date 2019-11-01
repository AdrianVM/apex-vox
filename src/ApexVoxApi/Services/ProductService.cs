using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApexVoxApi.Services
{
    public class ProductService:IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<Product> LoadData(Tenant tenant)
        {
            _unitOfWork.Migrate();

            for (var i = 1; i <= 10; i++)
            {
                var product = CreateProduct(tenant.Name, tenant.Id);

                if (!_unitOfWork.Products.Any(x => x.Name.Equals(product.Name)))
                {
                    _unitOfWork.Products.Add(product);
                }
            }

            _unitOfWork.SaveChanges();

            var products = _unitOfWork.Products.ToList();

            return products;
        }

        private Product CreateProduct(string tenantName, long tenantId)
        {
            return new Product()
            {
                Name = "Product_" + tenantName,
                TenantId = tenantId
            };
        }
    }
}
