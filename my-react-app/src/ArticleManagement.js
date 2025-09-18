import React, { useEffect, useState } from 'react';
import './ArticleManagement.css';
import { fetchArticles, fetchCategories } from './api'; // Assuming these functions exist in api.js

const ArticleManagement = ({ onSelectArticles, onClose, initialSelectedArticles = [] }) => {
  const [articles, setArticles] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [currentSelections, setCurrentSelections] = useState(initialSelectedArticles.map(article => article.id));

  useEffect(() => {
    loadArticles();
    loadCategories();
  }, []);

  // Update currentSelections if initialSelectedArticles changes (e.g., when modal re-opens)
  useEffect(() => {
    setCurrentSelections(initialSelectedArticles.map(article => article.id));
  }, [initialSelectedArticles]);

  const loadArticles = async () => {
    try {
      const data = await fetchArticles();
      setArticles(data);
    } catch (error) {
      console.error('Error fetching articles:', error);
    }
  };

  const loadCategories = async () => {
    try {
      const data = await fetchCategories();
      setCategories(data);
    } catch (error) {
      console.error('Error fetching categories:', error);
    }
  };

  const handleSearch = async () => {
    try {
      let filteredArticles = await fetchArticles();

      if (selectedCategory) {
        filteredArticles = filteredArticles.filter(article => article.category === selectedCategory);
      }
      if (searchTerm) {
        filteredArticles = filteredArticles.filter(article =>
          article.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
          article.content.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }
      setArticles(filteredArticles);

    } catch (error) {
      console.error('Error during search:', error);
    }
  };

  const handleReset = () => {
    setSelectedCategory('');
    setSearchTerm('');
    loadArticles();
  };

  const handleArticleSelect = (articleId) => {
    setCurrentSelections(prevSelections =>
      prevSelections.includes(articleId)
        ? prevSelections.filter(id => id !== articleId)
        : [...prevSelections, articleId]
    );
  };

  const handleConfirmSelection = () => {
    const confirmedArticles = articles.filter(article => currentSelections.includes(article.id));
    onSelectArticles(confirmedArticles);
  };

  const isAllSelected = articles.length > 0 && currentSelections.length === articles.length;

  const handleSelectAllChange = (e) => {
    if (e.target.checked) {
      setCurrentSelections(articles.map(article => article.id));
    } else {
      setCurrentSelections([]);
    }
  };

  return (
    <div className="article-management-container">
      <h2>Chọn nội dung bài viết ({currentSelections.length} đã chọn)</h2>

      <div className="filter-section">
        <select
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
          className="category-select"
        >
          <option value="">Tất cả danh mục</option>
          {categories.map((category) => (
            <option key={category.id} value={category.name}>
              {category.name}
            </option>
          ))}
        </select>
        <input
          type="text"
          placeholder="Tìm kiếm theo tiêu đề hoặc nội dung..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <button onClick={handleSearch} className="search-button">Tìm kiếm</button>
        <button onClick={handleReset} className="reset-button">Đặt lại</button>
      </div>

      <div className="article-list">
        {articles.length > 0 ? (
          <table>
            <thead>
              <tr>
                <th>
                  <input
                    type="checkbox"
                    onChange={handleSelectAllChange}
                    checked={isAllSelected}
                  />
                </th>
                <th>ID</th>
                <th>Tiêu đề</th>
                <th>Danh mục</th>
                <th>Ngày tạo</th>
              </tr>
            </thead>
            <tbody>
              {articles.map((article) => (
                <tr key={article.id}>
                  <td>
                    <input
                      type="checkbox"
                      checked={currentSelections.includes(article.id)}
                      onChange={() => handleArticleSelect(article.id)}
                    />
                  </td>
                  <td>{article.id}</td>
                  <td>{article.title}</td>
                  <td>{article.category}</td>
                  <td>{new Date(article.createdAt).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>Không có bài viết nào để hiển thị.</p>
        )}
      </div>
      <div className="modal-actions">
        <button onClick={handleConfirmSelection} className="confirm-selection-button">Xác nhận</button>
        <button onClick={onClose} className="cancel-button">Hủy</button>
      </div>
    </div>
  );
};

export default ArticleManagement;
