
import React, { useState, useEffect } from 'react';
import api from './api'; // Assuming you have an api.js file to handle communication with C#

const PostArticle = () => {
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [accounts, setAccounts] = useState([]);

    useEffect(() => {
        // Fetch categories of type 'account' when the component mounts
        const fetchCategories = async () => {
            try {
                const response = await api.get('/categories?type=account'); // You'll need to implement this on the C# side
                setCategories(response.data);
            } catch (error) {
                console.error("Error fetching categories:", error);
            }
        };
        fetchCategories();
    }, []);

    useEffect(() => {
        // Fetch accounts when a category is selected
        if (selectedCategory) {
            const fetchAccounts = async () => {
                try {
                    const response = await api.get(`/categories/${selectedCategory}/accounts`); // You'll need to implement this on the C# side
                    setAccounts(response.data);
                } catch (error) {
                    console.error("Error fetching accounts:", error);
                }
            };
            fetchAccounts();
        } else {
            setAccounts([]);
        }
    }, [selectedCategory]);

    return (
        <div>
            <h2>Đăng bài viết</h2>
            <div>
                <label htmlFor="category-select">Chọn danh mục:</label>
                <select 
                    id="category-select" 
                    value={selectedCategory} 
                    onChange={(e) => setSelectedCategory(e.target.value)}
                >
                    <option value="">--Chọn một danh mục--</option>
                    {categories.map(category => (
                        <option key={category.id} value={category.id}>
                            {category.name}
                        </option>
                    ))}
                </select>
            </div>

            {selectedCategory && (
                <div>
                    <h3>Tài khoản trong danh mục</h3>
                    <table>
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Tên</th>
                                {/* Add other account properties you want to display */}
                            </tr>
                        </thead>
                        <tbody>
                            {accounts.map(account => (
                                <tr key={account.id}>
                                    <td>{account.id}</td>
                                    <td>{account.name}</td>
                                    {/* Render other account properties */}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default PostArticle;
