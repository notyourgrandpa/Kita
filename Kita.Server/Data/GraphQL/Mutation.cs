using Kita.Application.DTOs;
using Kita.Domain.Categories;
using Kita.Domain.Products;
using Kita.Infrastructure.Persistence;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Server.Data.GraphQL
{
    public class Mutation
    {
        /// <summary>
        /// Add a new Product
        /// </summary>
        [Serial]
        //[Authorize(Roles = ["RegisteredUser"])]
        public async Task<Product> AddProduct(
            [Service] ApplicationDbContext context, ProductCreateDTO productDTO)
        {
            var product = new Product()
            {
                Name = productDTO.Name,
                ProductCode = productDTO.ProductCode,
                SellingPrice = productDTO.SellingPrice,
                CostPrice = productDTO.CostPrice,
                IsActive = productDTO.IsActive,
                CategoryId = productDTO.CategoryId
            };
            product.SetCreated(DateTime.UtcNow);
            context.Products.Add(product);
            await context.SaveChangesAsync();
            return product;
        }
        /// <summary>
        /// Update an existing Product
        /// </summary>
        [Serial]
        //[Authorize(Roles = ["RegisteredUser"])]
        public async Task<Product> UpdateProduct(
            [Service] ApplicationDbContext context, ProductUpdateDTO productDTO)
        {
            var product = await context.Products
                           .Where(c => c.Id == productDTO.Id)
                           .FirstOrDefaultAsync();
            if (product == null)
            {
                // todo: handle errors
                throw new NotSupportedException();
            }

            product.Name = productDTO.Name;
            product.SellingPrice = productDTO.SellingPrice;
            product.CostPrice = productDTO.CostPrice;
            product.IsActive = productDTO.IsActive;
            product.CategoryId = productDTO.CategoryId;
            product.SetUpdated(DateTime.UtcNow);
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return product;
        }
        /// <summary>
        /// Delete a Product
        /// </summary>
        [Serial]
        [Authorize(Roles = ["Administrator"])]
        public async Task DeleteProduct(
            [Service] ApplicationDbContext context, int id)
        {
            var product = await context.Products
                           .Where(c => c.Id == id)
                           .FirstOrDefaultAsync();
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Add a new Category
        /// </summary>
        [Serial]
        //[Authorize(Roles = ["RegisteredUser"])]
        public async Task<Category> AddCategory(
            [Service] ApplicationDbContext context, CategoryCreateInputDTO categoryDTO)
        {
            var category = new Category()
            {
                Name = categoryDTO.Name,
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }
        /// <summary>
        /// Update an existing Category
        /// </summary>
        [Serial]
        //[Authorize(Roles = ["RegisteredUser"])]
        public async Task<Category> UpdateCategory(
            [Service] ApplicationDbContext context, CategoryUpdateDTO categoryDTO)
        {
            var category = await context.Categories
                           .Where(c => c.Id == categoryDTO.Id)
                           .FirstOrDefaultAsync();
            if (category == null)
                // todo: handle errors
                throw new NotSupportedException();
            category.Name = categoryDTO.Name;
            context.Categories.Update(category);
            await context.SaveChangesAsync();
            return category;
        }
        /// <summary>
        /// Delete a Category
        /// </summary>
        [Serial]
        [Authorize(Roles = ["Administrator"])]
        public async Task DeleteCategory(
            [Service] ApplicationDbContext context, int id)
        {
            var category = await context.Categories
                           .Where(c => c.Id == id)
                           .FirstOrDefaultAsync();
            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }
        }
    }
}
