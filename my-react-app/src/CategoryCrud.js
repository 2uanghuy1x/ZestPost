
import React, { useState, useEffect } from 'react';
import { csharpApi } from './api';
import './CategoryCrud.css'; // We will create this CSS file next

function CategoryCrud() {
    const [categories, setCategories] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentCategory, setCurrentCategory] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'actionSuccess') {
                csharpApi.getCategories();
                setIsModalOpen(false);
                setCurrentCategory(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getCategories();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentCategory({ name: '', type: '' });
        setIsModalOpen(true);
    };

    const handleEdit = (category) => {
        setCurrentCategory(category);
        setIsModalOpen(true);
    };

    const handleDelete = (category) => {
        if (window.confirm(`Are you sure you want to delete category ${category.name}?`)) {
            csharpApi.deleteCategory(category);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        if (currentCategory.id) {
            csharpApi.updateCategory(currentCategory);
        } else {
            csharpApi.addCategory(currentCategory);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentCategory({ ...currentCategory, [name]: value });
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Danh mục</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên danh mục</th>
                                <th>Loại</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {categories.map((category) => (
                                <tr key={category.id}>
                                    <td>{category.name}</td>
                                    <td>{category.type}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(category)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(category)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>{currentCategory && currentCategory.id ? 'Sửa danh mục' : 'Thêm danh mục'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <label>Tên danh mục:</label>
                            <input name="name" value={currentCategory ? currentCategory.name : ''} onChange={handleInputChange} required />
                            
                            <label>Loại:</label>
                            <input name="type" value={currentCategory ? currentCategory.type : ''} onChange={handleInputChange} />
                            
                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default CategoryCrud;
