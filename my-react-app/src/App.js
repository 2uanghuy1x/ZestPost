
import React, { useState } from 'react';
import AccountCrud from './mst/account/AccountCrud';
import CategoryCrud from './mst/category/CategoryCrud';
import ArticleCrud from './mst/mst-article/ArticleCrud';
import HistoryAccountCrud from './mst/history/HistoryAccountCrud';
import AccountPageGroupManager from './mst/page/AccountPageGroupManager'; 
import GroupAccountCrud from './mst/group/GroupAccountCrud';
import PostArticle from './action/post-article/PostArticle';
import ScanAccounts from './action/scan/ScanAccounts'; // Keep this import
import Sidebar from './Sidebar';
import './App.css';

function App() {
    const [currentView, setCurrentView] = useState('accounts');

    const handleNavigation = (view) => {
        setCurrentView(view);
    };

    let content;
    switch (currentView) {
        case 'accounts':
            content = <AccountCrud />;
            break;
        case 'categories':
            content = <CategoryCrud />;
            break;
        case 'articles':
            content = <ArticleCrud />;
            break;
        case 'history':
            content = <HistoryAccountCrud />;
            break;
        case 'pages': // This case now renders AccountPageGroupManager
            content = <AccountPageGroupManager />;
            break;
        case 'groups':
            content = <GroupAccountCrud />;
            break;
        case 'post-article':
            content = <PostArticle />;
            break;
        case 'scan-accounts': // Add new case for ScanAccounts
            content = <ScanAccounts />;
            break;
        default:
            content = <AccountCrud />;
    }

    return (
        <div className="app-container">
            <Sidebar onNavigate={handleNavigation} currentView={currentView} />
            <div className="main-app-content">
                {content}
            </div>
        </div>
    );
}

export default App;
