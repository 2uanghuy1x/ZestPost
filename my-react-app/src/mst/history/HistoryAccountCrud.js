import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './HistoryAccountCrud.css';

function HistoryAccountCrud() {
    const [historyAccounts, setHistoryAccounts] = useState([]);
    const [allHistoryAccounts, setAllHistoryAccounts] = useState([]); // Store all history accounts fetched
    const [accounts, setAccounts] = useState([]); // Store accounts to map category
    const [categories, setCategories] = useState([]); // Store categories
    const [selectedCategory, setSelectedCategory] = useState(''); // State for selected category filter

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'historyAccountsData') {
                setHistoryAccounts(message.payload);
                setAllHistoryAccounts(message.payload); // Store all history accounts
            } else if (message.action === 'accountsData') {
                setAccounts(message.payload);
            } else if (message.action === 'categoriesData') {
                setCategories(message.payload);
            }
            if (message.action === 'actionSuccess') {
                // Re-fetch all data after a successful action (e.g., deletion)
                csharpApi.getHistoryAccounts();
                csharpApi.getAccounts();
                csharpApi.getCategories();
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getHistoryAccounts(); // Initial fetch
        csharpApi.getAccounts(); // Fetch accounts to link with history
        csharpApi.getCategories(); // Fetch categories

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    // Effect to filter history accounts based on selectedCategory
    useEffect(() => {
        if (selectedCategory && accounts.length > 0 && categories.length > 0) {
            const filteredAccountIds = accounts
                .filter(account => account.categoryId === selectedCategory)
                .map(account => account.id);
            
            const filteredHistory = allHistoryAccounts.filter(history => 
                filteredAccountIds.includes(history.accountId)
            );
            setHistoryAccounts(filteredHistory);
        } else {
            setHistoryAccounts(allHistoryAccounts);
        }
    }, [selectedCategory, allHistoryAccounts, accounts, categories]);

    const handleDelete = (historyAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa mục lịch sử ${historyAccount.id} này không?`)) {
            csharpApi.deleteHistoryAccount(historyAccount);
        }
    };

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    const getAccountName = (accountId) => {
        const account = accounts.find(acc => acc.id === accountId);
        return account ? account.name : 'N/A';
    };

    const getAccountCategoryName = (accountId) => {
        const account = accounts.find(acc => acc.id === accountId);
        if (account && account.categoryId) {
            const category = categories.find(cat => cat.id === account.categoryId);
            return category ? category.name : 'N/A';
        }
        return 'N/A';
    };

    const accountCategories = categories.filter(c => c.type === 'account');

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Lịch sử Tài khoản</h3>
                </div>
                <div className="filter-section">
                    <label htmlFor="categoryFilter">Lọc theo danh mục tài khoản:</label>
                    <select id="categoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
                        <option value="">Tất cả danh mục</option>
                        {accountCategories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Tên Tài khoản</th>
                                <th>Danh mục Tài khoản</th>
                                <th>Hành động</th>
                                <th>Thời gian</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {historyAccounts.map((historyAccount) => (
                                <tr key={historyAccount.id}>
                                    <td>{historyAccount.id}</td>
                                    <td>{getAccountName(historyAccount.accountId)}</td>
                                    <td>{getAccountCategoryName(historyAccount.accountId)}</td>
                                    <td>{historyAccount.action}</td>
                                    <td>{new Date(historyAccount.timestamp).toLocaleString()}</td>
                                    <td className="actions-column">
                                        <button className="action-btn delete" onClick={() => handleDelete(historyAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default HistoryAccountCrud;
