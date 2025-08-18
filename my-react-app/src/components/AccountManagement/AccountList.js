import React, { useState, useEffect } from 'react';
import './AccountList.css';
import { csharpApi } from '../../api';

function AccountList() {
    const [accounts, setAccounts] = useState([]);
    const [allAccounts, setAllAccounts] = useState([]); // Store all accounts fetched
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedAccountIds, setSelectedAccountIds] = useState(new Set()); // New state for selected account IDs

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'accountsData') {
                setAllAccounts(message.payload);
                // After fetching new accounts, clear selection if previously selected accounts are no longer in the list
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
        csharpApi.getAccounts(); // Initial fetch
        csharpApi.getCategories();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);
    
    const accountCategories = categories.filter(c => c.type === 'account');

    useEffect(() => {
        let filteredAccounts = allAccounts;

        if (selectedCategory) {
            filteredAccounts = filteredAccounts.filter(account => account.categoryId === selectedCategory);
        }

        if (searchTerm) {
            filteredAccounts = filteredAccounts.filter(account =>
                account.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                account.uid.toLowerCase().includes(searchTerm.toLowerCase())
            );
        }

        setAccounts(filteredAccounts);
    }, [selectedCategory, searchTerm, allAccounts]);

    const handleCategoryFilterChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    const handleSearchTermChange = (e) => {
        setSearchTerm(e.target.value);
    };

    const handleSelectAccount = (accountId) => {
        setSelectedAccountIds(prevSelected => {
            const newSelected = new Set(prevSelected);
            if (newSelected.has(accountId)) {
                newSelected.delete(accountId);
            } else {
                newSelected.add(accountId);
            }
            return newSelected;
        });
    };

    const handleSelectAllAccounts = () => {
        if (selectedAccountIds.size === accounts.length && accounts.length > 0) {
            // If all visible accounts are selected, deselect all
            setSelectedAccountIds(new Set());
        } else {
            // Otherwise, select all visible accounts
            const allVisibleAccountIds = new Set(accounts.map(account => account.id));
            setSelectedAccountIds(allVisibleAccountIds);
        }
    };

    const handleStartSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để bắt đầu.');
            return;
        }
        const accountsToStart = Array.from(selectedAccountIds).map(id => accounts.find(acc => acc.id === id));
        alert(`Bắt đầu các tài khoản đã chọn: ${accountsToStart.map(a => a.name).join(', ')}`);
        // TODO: Call csharpApi.startAccounts(Array.from(selectedAccountIds));
        // For now, let's just clear selection after action
        setSelectedAccountIds(new Set());
    };

    const handleStopSelected = () => {
        if (selectedAccountIds.size === 0) {
            alert('Vui lòng chọn ít nhất một tài khoản để dừng.');
            return;
        }
        const accountsToStop = Array.from(selectedAccountIds).map(id => accounts.find(acc => acc.id === id));
        alert(`Dừng các tài khoản đã chọn: ${accountsToStop.map(a => a.name).join(', ')}`);
        // TODO: Call csharpApi.stopAccounts(Array.from(selectedAccountIds));
        // For now, let's just clear selection after action
        setSelectedAccountIds(new Set());
    };

    const getCategoryName = (categoryId) => {
        const category = categories.find(c => c.id === categoryId);
        return category ? category.name : 'N/A';
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Danh sách Tài khoản</h3>
                </div>
                <div className="filter-section">
                    <div className="filter-group">
                        <label htmlFor="categoryFilter">Lọc theo danh mục:</label>
                        <select id="categoryFilter" value={selectedCategory} onChange={handleCategoryFilterChange}>
                            <option value="">Tất cả danh mục</option>
                            {accountCategories.map(category => (
                                <option key={category.id} value={category.id}>{category.name}</option>
                            ))}
                        </select>
                    </div>
                    <div className="filter-group">
                        <label htmlFor="accountSearch">Tìm tài khoản:</label>
                        <input
                            id="accountSearch"
                            type="text"
                            placeholder="Tìm theo tên hoặc UID..."
                            value={searchTerm}
                            onChange={handleSearchTermChange}
                        />
                    </div>
                    <div className="action-buttons">
                        <button
                            className="action-btn start"
                            onClick={handleStartSelected}
                            disabled={selectedAccountIds.size === 0}
                        >
                            Start Selected ({selectedAccountIds.size})
                        </button>
                        <button
                            className="action-btn stop"
                            onClick={handleStopSelected}
                            disabled={selectedAccountIds.size === 0}
                        >
                            Stop Selected ({selectedAccountIds.size})
                        </button>
                    </div>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>
                                    <input
                                        type="checkbox"
                                        onChange={handleSelectAllAccounts}
                                        checked={accounts.length > 0 && selectedAccountIds.size === accounts.length}
                                        // Indeterminate state for checkbox if some but not all are selected
                                        ref={input => {
                                            if (input) {
                                                input.indeterminate = selectedAccountIds.size > 0 && selectedAccountIds.size < accounts.length;
                                            }
                                        }}
                                    />
                                </th>
                                <th>Tên</th>
                                <th>UID</th>
                                <th>Avatar</th>
                                <th>Danh mục</th>
                            </tr>
                        </thead>
                        <tbody>
                            {accounts.map((account) => (
                                <tr key={account.id}>
                                    <td>
                                        <input
                                            type="checkbox"
                                            checked={selectedAccountIds.has(account.id)}
                                            onChange={() => handleSelectAccount(account.id)}
                                        />
                                    </td>
                                    <td>{account.name}</td>
                                    <td>{account.uid}</td>
                                    <td>
                                        {account.avatar ? (
                                            <img src={account.avatar} alt="Avatar" style={{ width: '50px', height: '50px', borderRadius: '50%', objectFit: 'cover' }} />
                                        ) : (
                                            'N/A'
                                        )}
                                    </td>
                                    <td>{getCategoryName(account.categoryId)}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default AccountList;
