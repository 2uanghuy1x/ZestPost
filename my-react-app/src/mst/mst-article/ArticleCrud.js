import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import '../category/CategoryCrud.css';

function ArticleCrud() {
    const [articles, setArticles] = useState([]);
    const [allArticles, setAllArticles] = useState([]); // Store all articles fetched
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentArticle, setCurrentArticle] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'articlesData') {
                setArticles(message.payload);
                setAllArticles(message.payload); // Store all articles
            } else if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }

            if (message.action === 'actionSuccess') {
                setIsModalOpen(false);
                setCurrentArticle(null);
                // Re-fetch all articles to update the list after a CRUD operation
                csharpApi.getArticles(); 
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getArticles(); // Initial fetch of all articles
        csharpApi.getCategories(); // Fetch all categories

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    useEffect(() => {
        if (selectedCategory) {
            const filtered = allArticles.filter(article => article.categoryId === selectedCategory);
            setArticles(filtered);
        } else {
            setArticles(allArticles);
        }
    }, [selectedCategory, allArticles]);

    const handleAddNew = () => {
        setCurrentArticle({ title: '', content: '', categoryId: '' });
        setIsModalOpen(true);
    };

    const handleEdit = (article) => {
        setCurrentArticle(article);
        setIsModalOpen(true);
    };

    const handleDelete = (article) => {
        if (window.confirm(`Are you sure you want to delete article "${article.title}"?`)) {
            csharpApi.deleteArticle(article);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        if (currentArticle.id) {
            csharpApi.updateArticle(currentArticle);
        } else {
            csharpApi.addArticle(currentArticle);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentArticle({ ...currentArticle, [name]: value });
    };

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Nội dung Bài viết</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="filter-section">
                    <label htmlFor="categoryFilter">Lọc theo danh mục:</label>
                    <select id="categoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
                        <option value="">Tất cả danh mục</option>
                        {categories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tiêu đề</th>
                                <th>Danh mục</th>
                                <th>Nội dung (xem trước)</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {articles.map((article) => (
                                <tr key={article.id}>
                                    <td>{article.title}</td>
                                    <td>{categories.find(cat => cat.id === article.categoryId)?.name || 'Không có'}</td>
                                    <td>{`${article.content.substring(0, 100)}...`}</td>
                                    <td className="actions-column">
                                        <button className="action-btn edit" onClick={() => handleEdit(article)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(article)}>Xóa</button>
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
                            <h2>{currentArticle && currentArticle.id ? 'Sửa bài viết' : 'Thêm bài viết'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <label>Tiêu đề:</label>
                            <input name="title" value={currentArticle?.title || ''} onChange={handleInputChange} required />
                            
                            <label>Danh mục:</label>
                            <select name="categoryId" value={currentArticle?.categoryId || ''} onChange={handleInputChange} required>
                                <option value="">Chọn danh mục</option>
                                {categories.map(category => (
                                    <option key={category.id} value={category.id}>{category.name}</option>
                                ))}
                            </select>

                            <label>Nội dung:</label>
                            <textarea 
                                name="content" 
                                value={currentArticle?.content || ''} 
                                onChange={handleInputChange} 
                                rows="10"
                                style={{ width: '100%', boxSizing: 'border-box', marginBottom: '20px', padding: '10px' }}
                                required 
                            />
                            
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

export default ArticleCrud;
