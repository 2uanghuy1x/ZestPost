import React, { useState, useEffect } from 'react';
import './AccountList.css';
import { csharpApi, fetchAccounts, fetchCategories } from '../../api';

function AccountList() {
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

    return (
        <div className="account-management-container">
            <h2 className="header-title">DANH SÁCH TÀI KHOẢN</h2>

            <div className="controls-row">
                <div className="category-select-wrapper">
                    <select
                        value={selectedCategory}
                        onChange={handleCategoryChange}
                        className="category-select"
                    >
                        <option value="">Chọn danh mục tài khoản</option>
                        {accountCategories.map(category => (
                            <option key={category.id} value={category.id}>{category.name}</option>
                        ))}
                    </select>
                </div>
                <button onClick={handleLoadAccounts} className="load-button">LOAD</button>
                <button onClick={handleStartSelected} className="start-button">START</button>
                <button onClick={handleStopSelected} className="stop-button">STOP</button>
            </div>

            <div className="account-list-table">
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
        </div>
    );
}

export default AccountList;
