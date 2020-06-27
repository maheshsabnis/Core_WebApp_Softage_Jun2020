using Core_WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.Services
{
    public class ProductRepository : IRepository<Product, int>
    {
        private readonly AppJune2020DbContext ctx;

        public ProductRepository(AppJune2020DbContext ctx)
        {
            this.ctx = ctx; // the ctx that is injected from DI
        }

        public async Task<Product> CraeteAsync(Product entity)
        {
            var res = await ctx.Products.AddAsync(entity);
            await ctx.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var prd = await ctx.Products.FindAsync(id);
            if (prd != null)
            {
                ctx.Products.Remove(prd);
                await ctx.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Product>> GetAsync()
        {
            return await ctx.Products.ToListAsync();
        }

        public async Task<Product> GetAsync(int id)
        {
            return await ctx.Products.FindAsync(id);
        }

        public async Task<Product> UpdateAsync(int id, Product entity)
        {
             // C# 7.0+ new declaratrion knonw as 'Discard'
            var _ = await ctx.Products.FindAsync(id);

            if (_!= null)
            {
                _.ProductId = entity.ProductId;
                _.ProductName = entity.ProductName;
                _.Description = entity.Description;
                _.Price = entity.Price;
                _.CategoryRowId = entity.CategoryRowId;
                await ctx.SaveChangesAsync();
                return _;
            }

            return entity;
        }
    }
}
