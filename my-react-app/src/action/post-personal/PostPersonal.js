import React, { useState, useEffect } from 'react';
import './PostPersonal.css';
import { csharpApi, fetchAccounts, fetchCategories } from '../../api';
import ArticleManagement from '../../ArticleManagement'; // Import the ArticleManagement component

const PostPersonal = () => {
    const [isArticleManagementOpen, setIsArticleManagementOpen] = useState(false);
    const [selectedArticles, setSelectedArticles] = useState([]);
    const [generalConfig, setGeneralConfig] = useState({});
    const [contentConfig, setContentConfig] = useState({});

    // Account management states
    const [accounts, setAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [selectedAccountIds, setSelectedAccountIds] = useState(new Set());

    useEffect(() => {
        loadCategories();
        loadAccounts(); // Load accounts on initial render

        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAccounts(message.payload);
                setSelectedAccountIds(prevSelected => {
                    const newSelected = new Set();
                    message.payload.forEach(account => {
                        if (prevSelected.has(account.id)) {
                            newSelected.add(account.id);
                        }
                    });
                    return newSelected;
                });
            }
            if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
        };

        csharpApi.addEventListener('message', handleMessage);

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const loadAccounts = async () => {
        try {
            const data = await fetchAccounts();
            let filteredAccounts = data;
            if (selectedCategory) {
                filteredAccounts = data.filter(account => account.categoryId === selectedCategory);
            }
            setAccounts(filteredAccounts);
        } catch (error) {
            console.error('Error fetching accounts:', error);
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

    const handleLoadAccounts = () => {
        loadAccounts();
    };

    const handleCategoryChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    useEffect(() => {
        loadAccounts(); // Reload accounts when category changes
    }, [selectedCategory]);

    const handleSelectAccount = (accountId) => {
        setSelectedAccountIds(prevSelected => {
            const newSelected = new Set(prevSelected);
            if (newSelected.has(accountId)) newSelected.delete(accountId);
            else newSelected.add(accountId);
            return newSelected;
        });
    };

    const handleSelectAllAccounts = () => {
        if (selectedAccountIds.size === accounts.length && accounts.length > 0) {
            setSelectedAccountIds(new Set());
        } else {
            const allVisibleAccountIds = new Set(accounts.map(account => account.id));
            setSelectedAccountIds(allVisibleAccountIds);
        }
    };

    const handleStartSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để bắt đầu.');
            return;
        }
        csharpApi.startAccounts(Array.from(selectedAccountIds));
        // Optionally, clear selection after starting
        // setSelectedAccountIds(new Set());
    };

    const handleStopSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để dừng.');
            return;
        }
        csharpApi.stopAccounts(Array.from(selectedAccountIds));
        // Optionally, clear selection after stopping
        // setSelectedAccountIds(new Set());
    };

    const accountCategories = categories.filter(c => c.type === 'account');

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
                        <button className="icon-btn" onClick={handleLoadAccounts}>🔄</button>
                        <select
                            className="category-select"
                            value={selectedCategory}
                            onChange={handleCategoryChange}
                        >
                            <option value="">Chọn danh mục tài khoản</option>
                            {accountCategories.map(category => (
                                <option key={category.id} value={category.id}>{category.name}</option>
                            ))}
                        </select>
                        <button className="btn btn-load" onClick={handleLoadAccounts}>LOAD</button>
                        <div className="spacer"></div>
                        <button className="btn btn-start" onClick={handleStartSelected}>START</button>
                        <button className="btn btn-stop" onClick={handleStopSelected}>STOP</button>
                    </div>

                    <div className="table-wrapper">
                        {accounts.length > 0 ? (
                            <table>
                                <thead>
                                    <tr>
                                        <th>
                                            <input
                                                type="checkbox"
                                                onChange={handleSelectAllAccounts}
                                                checked={accounts.length > 0 && selectedAccountIds.size === accounts.length}
                                                ref={input => {
                                                    if (input) {
                                                        input.indeterminate = selectedAccountIds.size > 0 && selectedAccountIds.size < accounts.length;
                                                    }
                                                }}
                                            />
                                        </th>
                                        <th>STT</th>
                                        <th>UID</th>
                                        <th>Trạng thái</th>
                                        <th>Thành công</th>
                                        <th>Tình trạng</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {accounts.map((account, index) => (
                                        <tr key={account.id}>
                                            <td>
                                                <input
                                                    type="checkbox"
                                                    checked={selectedAccountIds.has(account.id)}
                                                    onChange={() => handleSelectAccount(account.id)}
                                                />
                                            </td>
                                            <td>{index + 1}</td>
                                            <td>{account.uid}</td>
                                            <td>{account.status}</td>
                                            <td>{account.successCount}</td>
                                            <td>{account.message}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        ) : (
                            <p>Không có tài khoản nào để hiển thị.</p>
                        )}
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
