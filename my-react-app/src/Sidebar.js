
import React from 'react';
import './Sidebar.css';

const Sidebar = ({ onNavigate, currentView }) => {
    return (
        <div className="sidebar">
            <div className="sidebar-header">
                ZestPost
            </div>
            <ul className="sidebar-nav">
                <li className="nav-title">Quản lý</li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'accounts' ? 'active' : ''}`}
                        onClick={() => onNavigate('accounts')}
                    >
                        <span className="nav-text">Tài khoản</span>
                    </a>
                </li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'categories' ? 'active' : ''}`}
                        onClick={() => onNavigate('categories')}
                    >
                        <span className="nav-text">Danh mục</span>
                    </a>
                </li>
                 <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'articles' ? 'active' : ''}`}
                        onClick={() => onNavigate('articles')}
                    >
                        <span className="nav-text">Nội dung bài viết</span>
                    </a>
                </li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'history' ? 'active' : ''}`}
                        onClick={() => onNavigate('history')}
                    >
                        <span className="nav-text">Lịch sử tài khoản</span>
                    </a>
                </li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'pages' ? 'active' : ''}`}
                        onClick={() => onNavigate('pages')}
                    >
                        <span className="nav-text">Tài khoản Trang</span>
                    </a>
                </li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'groups' ? 'active' : ''}`}
                        onClick={() => onNavigate('groups')}
                    >
                        <span className="nav-text">Tài khoản Nhóm</span>
                    </a>
                </li>
                <li className="nav-title">Tác vụ</li>
                <li className="nav-item">
                    <a 
                        href="#" 
                        className={`nav-link ${currentView === 'post-article' ? 'active' : ''}`}
                        onClick={() => onNavigate('post-article')}
                    >
                        <span className="nav-text">Đăng bài</span>
                    </a>
                </li>
            </ul>
        </div>
    );
};

export default Sidebar;
