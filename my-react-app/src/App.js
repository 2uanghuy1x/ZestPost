
import React, { useState } from 'react';
import './App.css';
import Sidebar from './Sidebar';
import AccountList from './components/AccountManagement/AccountList';
import CategoryCrud from './mst/category/CategoryCrud';
import ArticleCrud from './mst/mst-article/ArticleCrud';
import HistoryAccountCrud from './mst/history/HistoryAccountCrud';
import GroupAccountCrud from './mst/group/GroupAccountCrud';
import AccountPageGroupManager from './mst/page/AccountPageGroupManager';
import PostArticle from './action/post-article/PostArticle';
import JoinGroup from './action/join-group/JoinGroup';
import ScanAccounts from './action/scan/ScanAccounts';
import Settings from './components/settings/Settings';

function App() {
    const [currentView, setCurrentView] = useState('accounts');

    const handleNavigate = (view) => {
        console.log(`Navigating to: ${view}`); // Debugging line
        setCurrentView(view);
    };

    const renderContent = () => {
        switch (currentView) {
            case 'accounts':
                return <AccountList />;
            case 'categories':
                return <CategoryCrud />;
            case 'articles':
                return <ArticleCrud />;
            case 'history':
                return <HistoryAccountCrud />;
            case 'groups':
                return <GroupAccountCrud />;
            case 'pages':
                return <AccountPageGroupManager />;
            case 'post-article':
                return <PostArticle />;
            case 'join-group':
                return <JoinGroup />;
            case 'scan-accounts':
                return <ScanAccounts />;
            case 'settings':
                return <Settings />;
            default:
                return <AccountList />;
        }
    };

    return (
        <div className="App">
            <Sidebar onNavigate={handleNavigate} currentView={currentView} />
            <div className="main-content">
                {renderContent()}
            </div>
        </div>
    );
}

export default App;
