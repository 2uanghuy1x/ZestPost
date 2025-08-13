
import React, { useState, useEffect } from 'react';
import { csharpApi } from './api';
import './CategoryCrud.css'; // Reuse the same CSS for consistency

function ArticleCrud() {
    const [articles, setArticles] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentArticle, setCurrentArticle] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'articlesData') {
                setArticles(message.payload);
            }
            if (message.action === 'actionSuccess') {
                csharpApi.getArticles(); // Assumes this will be added to the API
                setIsModalOpen(false);
                setCurrentArticle(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getArticles(); // Assumes this will be added to the API

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentArticle({ title: '', content: '' });
        setIsModalOpen(true);
    };

    const handleEdit = (article) => {
        setCurrentArticle(article);
        setIsModalOpen(true);
    };

    const handleDelete = (article) => {
        if (window.confirm(`Are you sure you want to delete article "${article.title}"?`)) {
            csharpApi.deleteArticle(article); // Assumes this will be added to the API
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        if (currentArticle.id) {
            csharpApi.updateArticle(currentArticle); // Assumes this will be added
        } else {
            csharpApi.addArticle(currentArticle); // Assumes this will be added
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentArticle({ ...currentArticle, [name]: value });
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Nội dung Bài viết</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tiêu đề</th>
                                <th>Nội dung (xem trước)</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {articles.map((article) => (
                                <tr key={article.id}>
                                    <td>{article.title}</td>
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
