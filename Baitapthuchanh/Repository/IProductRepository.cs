﻿using System.Collections.Generic;
using Baitapthuchanh.Models;
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    
}