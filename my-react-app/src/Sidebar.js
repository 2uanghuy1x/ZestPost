
import React from 'react';
import './Sidebar.css';
import Accordion from './components/Accordion/Accordion';

const Sidebar = ({ onNavigate, currentView }) => {
    // Helper function to create navigation links
    const renderNavLink = (view, icon, text) => (
        <li className="nav-item">
            <button className={`nav-link ${currentView === view ? 'active' : ''}`} onClick={() => onNavigate(view)}>
                <i className={`fas ${icon}`}></i>
                <span className="nav-text">{text}</span>
            </button>
        </li>
    );

    return (
        <div className="sidebar">
            <div className="sidebar-header">
                <h1 className="logo">ZestPost</h1>
            </div>
            <ul className="nav-menu">
                <Accordion title="Quản lý">
                    {renderNavLink('accounts', 'fa-users', 'Tài khoản')}
                    {renderNavLink('categories', 'fa-tags', 'Danh mục')}
                    {renderNavLink('articles', 'fa-newspaper', 'Nội dung bài viết')}
                    {renderNavLink('history', 'fa-history', 'Lịch sử')}
                    {renderNavLink('groups', 'fa-user-friends', 'Nhóm')}
                    {renderNavLink('pages', 'fa-flag', 'Trang')}
                </Accordion>
                <Accordion title="Hành động">
                    {renderNavLink('post-article', 'fa-file-alt', 'Đăng bài')}
                    {renderNavLink('post-personal', 'fa-file-alt', 'Đăng bài cá nhân')}
                    {renderNavLink('post-article-group-page', 'fa-file-alt', 'Đăng bài nhóm Page')}
                    {renderNavLink('post-article-group-regular', 'fa-file-alt', 'Đăng bài nhóm')}
                    {renderNavLink('join-group', 'fa-users-cog', 'Xin vào nhóm')}
                    {renderNavLink('scan-accounts', 'fa-search', 'Quét tài khoản')}
                </Accordion>
            </ul>
            <div className="sidebar-footer">
                 <button className={`nav-link ${currentView === 'settings' ? 'active' : ''}`} onClick={() => onNavigate('settings')}>
                    <i className="fas fa-cog"></i>
                    <span className="nav-text">Cài đặt</span>
                </button>
            </div>
        </div>
    );
};

export default Sidebar;
