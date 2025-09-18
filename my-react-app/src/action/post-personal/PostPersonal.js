import React, { useState, useEffect } from 'react';
import './PostPersonal.css';
import { csharpApi } from '../../api';
import ArticleManagement from '../../ArticleManagement'; // Import the ArticleManagement component

const PostPersonal = () => {
    const [isArticleManagementOpen, setIsArticleManagementOpen] = useState(false);
    const [selectedArticles, setSelectedArticles] = useState([]);
    const [generalConfig, setGeneralConfig] = useState({});
    const [contentConfig, setContentConfig] = useState({});

    // Handler for opening the ArticleManagement modal
    const handleOpenArticleManagement = () => {
        setIsArticleManagementOpen(true);
    };

    // Handler for closing the ArticleManagement modal
    const handleCloseArticleManagement = () => {
        setIsArticleManagementOpen(false);
    };

    // Handler for when articles are selected in ArticleManagement
    const handleArticlesSelected = (articles) => {
        setSelectedArticles(articles);
        // Potentially update contentConfig with selected article content
        if (articles.length > 0) {
            // For simplicity, let's just take the content of the first selected article
            setContentConfig(prev => ({ ...prev, content: articles[0].content }));
        } else {
            setContentConfig(prev => ({ ...prev, content: '' }));
        }
        setIsArticleManagementOpen(false); // Close the modal after selection
    };

    const handleGeneralConfigChange = (e) => {
        setGeneralConfig({ ...generalConfig, [e.target.name]: e.target.value });
    };

    const handleContentConfigChange = (e) => {
        setContentConfig({ ...contentConfig, [e.target.name]: e.target.value });
    };

    return (
        <div className="post-personal-container">
            <div className="config-panel">
                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">CẤU HÌNH ĐĂNG BÀI LÊN TRANG CÁ NHÂN</legend>
                    <button type="button" className="guideline-link">Xem hướng dẫn tại đây...</button>
                    
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="threads">⚙️</span> Số luồng chạy đồng thời <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input type="number" defaultValue="1" className="small-input" />
                            <span>luồng</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="max-posts">📝</span> Mỗi tài khoản đăng tối đa</label>
                        <div className="input-group">
                            <input type="number" defaultValue="0" className="small-input" />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="switch-error">🔄</span> Chuyển tài khoản nếu đăng lỗi <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input type="number" defaultValue="1" className="small-input" />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="switch-after-post">➡️</span> Chuyển tài khoản sau khi đăng</label>
                        <div className="input-group">
                            <input type="number" defaultValue="1" className="small-input" />
                            <span>bài viết</span>
                        </div>
                    </div>
                    <div className="config-item">
                        <label className="config-label"><span role="img" aria-label="interval">⏱️</span> Khoảng cách 2 lần đăng liên tiếp <span className="tooltip">[?]</span></label>
                        <div className="input-group">
                            <input type="number" defaultValue="1" className="range-input" />
                            <span>-</span>
                            <input type="number" defaultValue="1" className="range-input" />
                            <span>giây</span>
                        </div>
                    </div>
                </fieldset>

                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">Cấu hình bài viết</legend>
                    <div className="config-row">
                        <label className="switch">
                            <input type="checkbox" defaultChecked />
                            <span className="slider round"></span>
                        </label>
                        <span>Cấu hình đăng chung cho tất cả các tài khoản</span>
                    </div>
                    <div className="content-selection">
                        <button className="btn btn-primary" onClick={handleOpenArticleManagement}><span role="img" aria-label="select-content">➕</span> Chọn nội dung</button>
                        <span>Tổng số nội dung chọn đăng: <strong>{selectedArticles.length}</strong></span>
                    </div>

                    <div className="radio-group config-row">
                        <label><input type="radio" name="post-order" defaultChecked /> <span role="img" aria-label="random-post">🔀</span> Đăng ngẫu nhiên bài viết</label>
                        <label><input type="radio" name="post-order" /> <span role="img" aria-label="sequential-post">🔢</span> Đăng theo thứ tự</label>
                    </div>

                    <div className="checkbox-group">
                        <label className="config-row"><input type="checkbox" defaultChecked /> <span role="img" aria-label="no-duplicate">🚫</span> Không đăng trùng bài viết giữa các tài khoản</label>
                        <div className="config-row config-row-inline">
                            <label><input type="checkbox" defaultChecked /> <span role="img" aria-label="image-upload-time">⏳</span> Thời gian chờ tải ảnh lên</label>
                            <input type="number" defaultValue="1" className="small-input" /> 
                            <span>giây</span>
                        </div>
                        <label className="config-row"><input type="checkbox" defaultChecked /> <span role="img" aria-label="background-image">🖼️</span> Kèm ảnh background khi đăng trạng thái</label>
                        <label className="config-row"><input type="checkbox" defaultChecked /> <span role="img" aria-label="comment-after-post">💬</span> Bình luận vào bài viết sau khi đăng thành công</label>
                    </div>
                    <button type="button" className="comment-guideline">Vui lòng nhập nội dung bình luận vào đây. Mỗi nội dung một dòng !</button>
                </fieldset>
            </div>

            <div className="account-panel">
                <fieldset className="panel-fieldset">
                    <legend className="panel-legend">DANH SÁCH TÀI KHOẢN</legend>
                    <div className="toolbar">
                        <button className="icon-btn">🔄</button>
                        <select className="category-select">
                            <option>Chọn danh mục tài khoản</option>
                        </select>
                        <button className="btn btn-load">LOAD</button>
                        <div className="spacer"></div>
                        <button className="btn btn-start">START</button>
                        <button className="btn btn-stop">STOP</button>
                    </div>

                    <div className="table-wrapper">
                        <table>
                            <thead>
                                <tr>
                                    <th><input type="checkbox" /></th>
                                    <th>STT</th>
                                    <th>UID</th>
                                    <th>Trạng thái</th>
                                    <th>Thành công</th>
                                    <th>Tình trạng</th>
                                </tr>
                            </thead>
                            <tbody>
                                {/* Example row */}
                                <tr>
                                    <td><input type="checkbox" /></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </fieldset>
            </div>

            {/* Article Management Modal */}
            {isArticleManagementOpen && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <button className="modal-close-button" onClick={handleCloseArticleManagement}>X</button>
                        <ArticleManagement 
                            onClose={handleCloseArticleManagement} 
                            onSelectArticles={handleArticlesSelected} 
                            // Pass current selected articles to ArticleManagement if you want to pre-select them
                            initialSelectedArticles={selectedArticles}
                        />
                    </div>
                </div>
            )}
        </div>
    );
};

export default PostPersonal;
